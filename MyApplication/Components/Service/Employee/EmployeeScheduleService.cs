using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Employee;

public sealed class EmployeeScheduleService
{
    private readonly IDbContextFactory<AomDbContext> _contextFactory;

    public EmployeeScheduleService(IDbContextFactory<AomDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<EmployeeFilterOptionsDto> GetFilterOptionsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var managers = await context.Managers
            .Where(x => x.IsActive == true)
            .Select(x => x.Employee.LastName + ", " + x.Employee.FirstName)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        var supervisors = await context.Supervisors
            .Where(x => x.IsActive == true)
            .Select(x => x.Employee.LastName + ", " + x.Employee.FirstName)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        var orgs = await context.Organizations
            .Where(x => x.IsActive == true)
            .Select(x => x.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        var subOrgs = await context.SubOrganizations
            .Where(x => x.IsActive == true)
            .Select(x => x.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return new EmployeeFilterOptionsDto
        {
            Managers = managers,
            Supervisors = supervisors,
            Organizations = orgs,
            SubOrganizations = subOrgs
        };
    }

    public async Task<ScheduleDashboardVm> GetDailySchedulesAsync(
        DateOnly date,
        TimeDisplayMode mode,
        string? query,
        bool activeOnly,
        bool agentsOnly,
        bool hideLoa,
        string? manager,
        string? supervisor,
        string? org,
        string? subOrg,
        CancellationToken ct = default)
    {
        if (date == DateOnly.MinValue) return new ScheduleDashboardVm { Date = date };

        using var ctx = await _contextFactory.CreateDbContextAsync();

        var rawData = await ctx.DetailedSchedule.AsNoTracking()
            .Where(x => x.ScheduleDate == date)
            .Include(x => x.ActivityType)
            .Include(x => x.ActivitySubType)
            .Include(x => x.AwsStatus)
            .ToListAsync(ct);

        var empIds = rawData.Select(x => x.EmployeeId).Distinct().ToList();
        var histories = await ctx.EmployeeHistory.AsNoTracking()
            .Where(h => empIds.Contains(h.EmployeeId))
            .Include(h => h.Site)
            .Include(h => h.Organization)
            .Include(h => h.SubOrganization)
            .Include(h => h.Supervisor).ThenInclude(s => s.Employee)
            .Include(h => h.Manager).ThenInclude(m => m.Employee)
            .Include(h => h.Employee)
            .ToListAsync(ct);

        var bestHistory = histories
            .GroupBy(h => h.EmployeeId)
            .Select(g => g.OrderByDescending(x => x.IsActive == true).ThenByDescending(x => x.EffectiveDate).First())
            .ToList();

        var filteredHistory = bestHistory.Where(h =>
        {
            if (activeOnly && h.IsActive != true) return false;
            if (hideLoa && (h.IsLoa == true || h.IsIntLoa == true)) return false;

            if (agentsOnly)
            {
                var allowedOrgIds = new[] { 2, 3, 4 };
                if (h.Organization == null || !allowedOrgIds.Contains(h.Organization.Id)) return false;

                var excludedSubOrgIds = new[] { 2, 3 };
                if (h.SubOrganization != null && excludedSubOrgIds.Contains(h.SubOrganization.Id)) return false;
            }

            if (!string.IsNullOrEmpty(manager))
            {
                var mName = h.Manager?.Employee != null ? $"{h.Manager.Employee.LastName}, {h.Manager.Employee.FirstName}" : "";
                if (!mName.Contains(manager, StringComparison.OrdinalIgnoreCase)) return false;
            }
            if (!string.IsNullOrEmpty(supervisor))
            {
                var sName = h.Supervisor?.Employee != null ? $"{h.Supervisor.Employee.LastName}, {h.Supervisor.Employee.FirstName}" : "";
                if (!sName.Contains(supervisor, StringComparison.OrdinalIgnoreCase)) return false;
            }
            if (!string.IsNullOrEmpty(org) && h.Organization?.Name != org) return false;
            if (!string.IsNullOrEmpty(subOrg) && h.SubOrganization?.Name != subOrg) return false;

            return true;
        }).ToList();

        var visibleEmpIds = filteredHistory.Select(x => x.EmployeeId).Distinct().ToList();

        var awsMappingsRaw = await ctx.Identifiers.AsNoTracking()
            .Where(x => visibleEmpIds.Contains(x.EmployeeId ?? 0) && !string.IsNullOrEmpty(x.Guid))
            .Select(x => new { x.EmployeeId, x.Guid })
            .ToListAsync(ct);

        var guidToEmpMap = new Dictionary<Guid, int>();
        foreach (var row in awsMappingsRaw)
        {
            if (row.EmployeeId.HasValue && Guid.TryParse(row.Guid, out Guid parsedGuid))
                if (!guidToEmpMap.ContainsKey(parsedGuid)) guidToEmpMap[parsedGuid] = row.EmployeeId.Value;
        }
        var distinctGuids = guidToEmpMap.Keys.ToList();

        var sqlQueryStart = date.AddDays(-1).ToDateTime(TimeOnly.MinValue);
        var sqlQueryEnd = date.AddDays(2).ToDateTime(TimeOnly.MinValue);

        List<AwsAgentActivityDto> awsActivities = new();
        if (distinctGuids.Any())
        {
            awsActivities = await GetAwsActivitiesAsync(ctx, distinctGuids, sqlQueryStart, sqlQueryEnd, ct);
        }

        var employeeAwsData = awsActivities
            .Where(a => guidToEmpMap.ContainsKey(a.AwsGuid))
            .ToLookup(a => guidToEmpMap[a.AwsGuid]);

        var dashboard = new ScheduleDashboardVm { Date = date };
        var groupedBySite = filteredHistory.GroupBy(x => x.Site?.SiteCode ?? "Unknown").OrderBy(g => g.Key);

        var globalDayStart = date.ToDateTime(TimeOnly.MinValue);
        var globalDayEnd = globalDayStart.AddHours(30);

        foreach (var siteGroup in groupedBySite)
        {
            var siteName = siteGroup.Key;
            var first = siteGroup.First();
            var siteTz = first.Site?.TimeZoneWindows ?? first.Site?.TimeZoneIana ?? "Eastern Standard Time";
            var siteVm = new SiteScheduleGroupVm { SiteName = siteName, TimeZoneId = siteTz };

            string targetTzId = mode switch
            {
                TimeDisplayMode.Eastern => "Eastern Standard Time",
                TimeDisplayMode.Mountain => "Mountain Standard Time",
                _ => siteTz
            };

            DateTime nowInTarget;
            try
            {
                var targetZone = TimeZoneInfo.FindSystemTimeZoneById(targetTzId);
                nowInTarget = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, targetZone);
            }
            catch { nowInTarget = DateTime.MaxValue; }

            foreach (var hist in siteGroup)
            {
                var empSchedules = rawData.Where(x => x.EmployeeId == hist.EmployeeId).OrderBy(x => x.StartTime).ToList();

                var empVm = new EmployeeDailyScheduleVm
                {
                    EmployeeId = hist.EmployeeId,
                    EmployeeName = hist.Employee != null ? $"{hist.Employee.LastName}, {hist.Employee.FirstName}" : $"#{hist.EmployeeId}",
                    OrganizationName = hist.Organization?.Name ?? "",
                    SubOrganizationName = hist.SubOrganization?.Name ?? "",
                    SupervisorName = hist.Supervisor?.Employee != null ? $"{hist.Supervisor.Employee.LastName}, {hist.Supervisor.Employee.FirstName}" : ""
                };

                DateTime validWindowStartEt;
                DateTime validWindowEndEt;

                if (empSchedules.Any())
                {
                    var minStart = empSchedules.Min(s => s.StartTime);
                    var maxEnd = empSchedules.Max(s => s.EndTime);
                    validWindowStartEt = minStart.AddHours(-8);
                    validWindowEndEt = maxEnd.AddHours(8);
                }
                else
                {
                    validWindowStartEt = Tz.FromSiteToEt(globalDayStart, targetTzId);
                    validWindowEndEt = Tz.FromSiteToEt(globalDayEnd, targetTzId);
                }

                var actualSegmentsForAlerts = new List<(DateTime Start, DateTime End)>();
                var offlineSegments = new List<(DateTime Start, DateTime End)>();
                var workingSegments = new List<(DateTime Start, DateTime End)>();

                foreach (var s in empSchedules)
                {
                    var sStart = Tz.FromEtToSite(s.StartTime, targetTzId);
                    var sEnd = Tz.FromEtToSite(s.EndTime, targetTzId);

                    if (s.AwsStatus?.Id == 1) offlineSegments.Add((sStart, sEnd));
                    else workingSegments.Add((sStart, sEnd));
                }

                var validWorkWindows = new List<(DateTime Start, DateTime End)>();
                if (workingSegments.Any())
                {
                    workingSegments.Sort((a, b) => a.Start.CompareTo(b.Start));
                    offlineSegments.Sort((a, b) => a.Start.CompareTo(b.Start));

                    foreach (var work in workingSegments)
                    {
                        var currentStart = work.Start;
                        var workEnd = work.End;

                        foreach (var off in offlineSegments)
                        {
                            if (off.End <= currentStart) continue;
                            if (off.Start >= workEnd) break;
                            if (off.Start > currentStart) validWorkWindows.Add((currentStart, off.Start));
                            if (off.End > currentStart) currentStart = off.End;
                        }
                        if (currentStart < workEnd) validWorkWindows.Add((currentStart, workEnd));
                    }
                }

                DateTime? minDt = null;
                DateTime? maxDt = null;

                foreach (var item in empSchedules)
                {
                    var localStart = Tz.FromEtToSite(item.StartTime, targetTzId);
                    var localEnd = Tz.FromEtToSite(item.EndTime, targetTzId);

                    if (minDt == null || localStart < minDt) minDt = localStart;
                    if (maxDt == null || localEnd > maxDt) maxDt = localEnd;

                    string colorClass = GetActivityColor(
                        item.ActivityType?.Name,
                        item.ActivitySubType?.Name,
                        item.AwsStatus?.Name);

                    empVm.Segments.Add(new ScheduleSegmentVm
                    {
                        ActivityTypeId = item.ActivityTypeId,
                        ActivitySubTypeId = item.ActivitySubTypeId,
                        ActivityName = item.ActivityType?.Name ?? "?",
                        SubActivityName = item.ActivitySubType?.Name ?? "-",
                        AwsStatusId = item.AwsStatus?.Id,
                        AwsStatusName = item.AwsStatus?.Name ?? "-",
                        Start = localStart,
                        End = localEnd,
                        IsImpacting = item.IsImpacting ?? false,
                        ColorClass = colorClass
                    });
                }
                empVm.ShiftString = (minDt.HasValue && maxDt.HasValue) ? $"{minDt.Value:HH:mm} - {maxDt.Value:HH:mm}" : "OFF";

                if (employeeAwsData.Contains(hist.EmployeeId))
                {
                    foreach (var act in employeeAwsData[hist.EmployeeId])
                    {
                        if (act.StartTime >= validWindowStartEt && act.StartTime < validWindowEndEt)
                        {
                            var actStart = Tz.FromEtToSite(act.StartTime, targetTzId);
                            var actEnd = Tz.FromEtToSite(act.EndTime, targetTzId);

                            string awsColor = GetActivityColor(null, null, act.CurrentAgentStatus);

                            empVm.AwsSegments.Add(new ScheduleSegmentVm
                            {
                                ActivityName = "AWS",
                                SubActivityName = act.CurrentAgentStatus,
                                AwsStatusName = act.CurrentAgentStatus,
                                Start = actStart,
                                End = actEnd,
                                IsImpacting = true,
                                ColorClass = awsColor
                            });

                            actualSegmentsForAlerts.Add((actStart, actEnd));
                        }
                    }
                }

                var mergedActuals = new List<(DateTime Start, DateTime End)>();
                if (actualSegmentsForAlerts.Any())
                {
                    actualSegmentsForAlerts.Sort((a, b) => a.Start.CompareTo(b.Start));
                    var currentStart = actualSegmentsForAlerts[0].Start;
                    var currentEnd = actualSegmentsForAlerts[0].End;

                    for (int i = 1; i < actualSegmentsForAlerts.Count; i++)
                    {
                        if (actualSegmentsForAlerts[i].Start <= currentEnd.AddMinutes(1))
                        {
                            if (actualSegmentsForAlerts[i].End > currentEnd) currentEnd = actualSegmentsForAlerts[i].End;
                        }
                        else
                        {
                            mergedActuals.Add((currentStart, currentEnd));
                            currentStart = actualSegmentsForAlerts[i].Start;
                            currentEnd = actualSegmentsForAlerts[i].End;
                        }
                    }
                    mergedActuals.Add((currentStart, currentEnd));
                }

                if (empVm.Segments.Any())
                {
                    var subtractMask = new List<(DateTime Start, DateTime End)>();
                    subtractMask.AddRange(mergedActuals);
                    subtractMask.AddRange(offlineSegments);
                    subtractMask.Sort((a, b) => a.Start.CompareTo(b.Start));

                    foreach (var plan in empVm.Segments)
                    {
                        if (plan.AwsStatusId == 1) continue;
                        var current = plan.Start;
                        var end = plan.End;

                        foreach (var mask in subtractMask)
                        {
                            if (mask.End <= current) continue;
                            if (mask.Start >= end) break;
                            if (mask.Start > current)
                            {
                                var alertStart = current;
                                var alertEnd = mask.Start;
                                if (alertStart < nowInTarget)
                                {
                                    if (alertEnd > nowInTarget) alertEnd = nowInTarget;
                                    if ((alertEnd - alertStart).TotalMinutes > 5)
                                    {
                                        empVm.AlertSegments.Add(new ScheduleSegmentVm
                                        {
                                            ActivityName = "Alert",
                                            SubActivityName = "Missing",
                                            Start = alertStart,
                                            End = alertEnd,
                                            IsImpacting = true
                                        });
                                    }
                                }
                            }
                            if (mask.End > current) current = mask.End;
                        }
                        if (current < end)
                        {
                            var alertStart = current;
                            var alertEnd = end;
                            if (alertStart < nowInTarget)
                            {
                                if (alertEnd > nowInTarget) alertEnd = nowInTarget;
                                if ((alertEnd - alertStart).TotalMinutes > 5)
                                {
                                    empVm.AlertSegments.Add(new ScheduleSegmentVm
                                    {
                                        ActivityName = "Alert",
                                        SubActivityName = "Missing",
                                        Start = alertStart,
                                        End = alertEnd,
                                        IsImpacting = true
                                    });
                                }
                            }
                        }
                    }
                }

                if (mergedActuals.Any())
                {
                    foreach (var act in mergedActuals)
                    {
                        var current = act.Start;
                        var end = act.End;
                        foreach (var valid in validWorkWindows)
                        {
                            if (valid.End <= current) continue;
                            if (valid.Start >= end) break;
                            if (valid.Start > current)
                            {
                                var alertStart = current;
                                var alertEnd = valid.Start;
                                if (alertStart < nowInTarget)
                                {
                                    if (alertEnd > nowInTarget) alertEnd = nowInTarget;
                                    if ((alertEnd - alertStart).TotalMinutes > 5)
                                    {
                                        empVm.AlertSegments.Add(new ScheduleSegmentVm
                                        {
                                            ActivityName = "Alert",
                                            SubActivityName = "Unexpected",
                                            Start = alertStart,
                                            End = alertEnd,
                                            IsImpacting = true
                                        });
                                    }
                                }
                            }
                            if (valid.End > current) current = valid.End;
                        }
                        if (current < end)
                        {
                            var alertStart = current;
                            var alertEnd = end;
                            if (alertStart < nowInTarget)
                            {
                                if (alertEnd > nowInTarget) alertEnd = nowInTarget;
                                if ((alertEnd - alertStart).TotalMinutes > 5)
                                {
                                    empVm.AlertSegments.Add(new ScheduleSegmentVm
                                    {
                                        ActivityName = "Alert",
                                        SubActivityName = "Unexpected",
                                        Start = alertStart,
                                        End = alertEnd,
                                        IsImpacting = true
                                    });
                                }
                            }
                        }
                    }
                }

                siteVm.Employees.Add(empVm);
            }

            siteVm.Employees = siteVm.Employees.OrderBy(e => e.EmployeeName).ToList();
            if (siteVm.Employees.Any()) dashboard.Sites.Add(siteVm);
        }

        return dashboard;
    }

    private static string GetActivityColor(string? activityName, string? subActivityName, string? awsStatusName)
    {
        string nameToCheck = awsStatusName;

        if (string.IsNullOrEmpty(nameToCheck) || nameToCheck == "-")
        {
            nameToCheck = !string.IsNullOrEmpty(subActivityName) && subActivityName != "-"
                          ? subActivityName
                          : activityName;
        }

        var name = (nameToCheck ?? "").ToLower();

        if (name.Contains("lunch")) return "activity-lunch";
        if (name.Contains("engagement") || name.Contains("corp")) return "activity-corporate";
        if (name.Contains("peer")) return "activity-peer";
        if (name.Contains("meeting")) return "activity-meeting";
        if (name.Contains("training")) return "activity-training";
        if (name.Contains("coaching")) return "activity-coaching";

        if (name.Contains("customer") || name.Contains("phone") ||
            name.Contains("queue") || name.Contains("chat") ||
            name.Contains("email")) return "activity-ol-customer";

        if (name.Contains("admin")) return "activity-admin";
        if (name.Contains("system")) return "activity-system";
        if (name.Contains("available")) return "activity-available";

        if (name.Contains("break")) return "activity-break";
        if (name.Contains("offline")) return "activity-offline";

        return "activity-default";
    }

    private async Task<List<AwsAgentActivityDto>> GetAwsActivitiesAsync(
        AomDbContext db,
        List<Guid> visibleEmployeeAwsGuids,
        DateTime viewStart,
        DateTime viewEnd,
        CancellationToken ct = default)
    {
        if (!visibleEmployeeAwsGuids.Any()) return new List<AwsAgentActivityDto>();

        // Join the GUIDs into a string list for the SQL IN clause
        var guidList = string.Join("','", visibleEmployeeAwsGuids);

        // FIX: Use TRY_CAST([AwsGuid] AS UNIQUEIDENTIFIER) in the WHERE clause.
        // This ensures that the comparison is done as GUIDs, ignoring case (upper/lower)
        // and string format differences between the DB and C#.
        var sql = $@"
SELECT CAST([eventId] AS NVARCHAR(36)) as [EventId]
      ,CAST([awsId] AS NVARCHAR(36)) as [AwsId]
      ,[eventTimeET] as [StartTime]
      ,[currentAgentStatus]
      ,[endTime] as [EndTime]
      ,[duration]
      ,CAST([AwsGuid] AS UNIQUEIDENTIFIER) as [AwsGuid]
  FROM [TODNMCIAWS].[dbo].[awsDetailedAgentData]
  WHERE TRY_CAST([AwsGuid] AS UNIQUEIDENTIFIER) IN ('{guidList}')
    AND [eventTimeET] < {{0}}
    AND [endTime] > {{1}}
    AND [currentAgentStatus] != 'Offline'
  ORDER BY [eventTimeET]";

        return await db.Set<AwsAgentActivityDto>()
            .FromSqlRaw(sql, viewEnd, viewStart)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
