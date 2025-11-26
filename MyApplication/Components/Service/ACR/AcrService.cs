using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Common.Time;

namespace MyApplication.Components.Service.Acr
{
    public sealed class AcrService : IAcrService
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;
        private readonly IHttpContextAccessor _http;

        public AcrService(IDbContextFactory<AomDbContext> dbFactory, IHttpContextAccessor http)
        {
            _dbFactory = dbFactory;
            _http = http;
        }

        // Matches Employee.AcrStatus table
        private const int Submitted = 1;
        private const int ManagerApproved = 2;
        private const int WfmApproved = 3;
        private const int Cancelled = 5;
        private const int Rejected = 6;

        private string? CurrentUser => _http.HttpContext?.User?.Identity?.Name;

        public async Task<int> CreateAsync(AcrCreateVm vm, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var typeKey = (AcrTypeKey)vm.TypeId;

            // ---------- NEW HIRE BRANCH ----------
            if (typeKey == AcrTypeKey.NewHire)
            {
                if (vm.NewHire is null)
                    throw new InvalidOperationException("New Hire ACR missing NewHire data.");

                var nh = vm.NewHire;

                var emp = new Employees
                {
                    FirstName = nh.FirstName,
                    LastName = nh.LastName,
                    MiddleInitial = nh.MiddleInitial,
                    NmciEmail = nh.NmciEmail,
                    UsnOperatorId = nh.UsnOperatorId,
                    UsnAdminId = nh.UsnAdminId,
                    CorporateEmail = nh.CorporateEmail,
                    CorporateId = nh.CorporateId,
                    DomainLoginName = nh.DomainLoginName,
                    // NEW HIRE is always active
                    IsActive = true
                };

                db.Employees.Add(emp);
                await db.SaveChangesAsync(ct);   // emp.Id now populated

                // Point the ACR at the newly created employee
                vm = vm with { EmployeeId = emp.Id };
            }
            else
            {
                // For all non-NewHire types we must already have an EmployeeId
                if (vm.EmployeeId <= 0)
                    throw new InvalidOperationException("EmployeeId is required for this ACR type.");
            }

            // ---------- REHIRE / SEPARATION EMPLOYEE FLAGS ----------
            if (typeKey == AcrTypeKey.Rehire || typeKey == AcrTypeKey.Separation)
            {
                var emp = await db.Employees
                    .FirstOrDefaultAsync(e => e.Id == vm.EmployeeId, ct)
                    ?? throw new InvalidOperationException($"Employee {vm.EmployeeId} not found for {typeKey} ACR.");

                if (typeKey == AcrTypeKey.Rehire)
                {
                    // Rehire → mark active
                    emp.IsActive = true;
                }
                else if (typeKey == AcrTypeKey.Separation)
                {
                    // Separation → mark inactive
                    emp.IsActive = false;
                }
            }

            // ---------- Core ACR creation (for all types) ----------
            var user = CurrentUser; // same pattern as SetStatusAsync
            var acrId = await CreateAcrCoreAsync(db, vm, user, ct);
            return acrId;
        }

        private static async Task<int> CreateAcrCoreAsync(
            AomDbContext db,
            AcrCreateVm vm,
            string? submittedBy,
            CancellationToken ct)
        {
            // ---------- pick status id ----------
            int statusId;

            // New Hire, Rehire, Separation all start at WfmApproved (3)
            if (vm.TypeId == (int)AcrTypeKey.NewHire
                || vm.TypeId == (int)AcrTypeKey.Rehire
                || vm.TypeId == (int)AcrTypeKey.Separation)
            {
                statusId = 3;
            }
            else
            {
                statusId = await db.AcrStatuses
                    .Where(s => s.Name == "Submitted")
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync(ct);

                if (statusId == 0)
                    throw new InvalidOperationException("AcrStatus 'Submitted' not found.");
            }

            // Effective date – use the value from the VM if set; otherwise default to "today" in ET
            // REFACTOR: Use Et.Today
            var effectiveDate = vm.EffectiveDate ?? Et.Today;

            var req = new AcrRequest
            {
                EmployeeId = vm.EmployeeId,
                AcrTypeId = vm.TypeId,
                AcrStatusId = statusId,
                EffectiveDate = effectiveDate,
                SubmitterComment = vm.SubmitterComment,

                // Submit time stored in Eastern instead of UTC
                // REFACTOR: Use Et.Now
                SubmitTime = Et.Now,
                SubmittedBy = submittedBy,

                IsActive = vm.TypeId switch
                {
                    (int)AcrTypeKey.Separation => false,
                    (int)AcrTypeKey.Rehire => true,
                    (int)AcrTypeKey.NewHire => true,
                    _ => true
                }
            };

            db.AcrRequests.Add(req);
            await db.SaveChangesAsync(ct);

            var acrId = req.Id;

            // ---------- Organization ----------
            if (vm.Organization is not null)
            {
                var o = vm.Organization;
                db.AcrOrganizations.Add(new AcrOrganization
                {
                    AcrRequestId = acrId,
                    OrganizationId = o.OrganizationId,
                    SubOrganizationId = o.SubOrganizationId,
                    SiteId = o.SiteId,
                    EmployerId = o.EmployerId,
                    ManagerId = o.ManagerId,
                    SupervisorId = o.SupervisorId,
                    IsLoa = o.IsLoa,
                    IsIntLoa = o.IsIntLoa,
                    IsRemote = o.IsRemote
                });
            }

            // ---------- Schedule (already converted to ET on the page) ----------
            if (vm.Schedule is not null)
            {
                var s = vm.Schedule;
                db.AcrSchedules.Add(new AcrSchedule
                {
                    AcrRequestId = acrId,
                    ShiftNumber = s.ShiftNumber,
                    IsSplitSchedule = s.IsSplitSchedule,

                    MondayStart = s.MondayStart,
                    MondayEnd = s.MondayEnd,
                    TuesdayStart = s.TuesdayStart,
                    TuesdayEnd = s.TuesdayEnd,
                    WednesdayStart = s.WednesdayStart,
                    WednesdayEnd = s.WednesdayEnd,
                    ThursdayStart = s.ThursdayStart,
                    ThursdayEnd = s.ThursdayEnd,
                    FridayStart = s.FridayStart,
                    FridayEnd = s.FridayEnd,
                    SaturdayStart = s.SaturdayStart,
                    SaturdayEnd = s.SaturdayEnd,
                    SundayStart = s.SundayStart,
                    SundayEnd = s.SundayEnd,
                    IsStaticBreakSchedule = s.IsStaticBreakSchedule,
                    IsOtAdjustment = s.IsOtAdjustment
                });
            }

            // ---------- Overtime ----------
            if (vm.Overtime is not null)
            {
                var o = vm.Overtime;
                db.AcrOvertimeSchedules.Add(new AcrOvertimeSchedules
                {
                    AcrRequestId = acrId,
                    MondayTypeId = o.MondayTypeId,
                    TuesdayTypeId = o.TuesdayTypeId,
                    WednesdayTypeId = o.WednesdayTypeId,
                    ThursdayTypeId = o.ThursdayTypeId,
                    FridayTypeId = o.FridayTypeId,
                    SaturdayTypeId = o.SaturdayTypeId,
                    SundayTypeId = o.SundayTypeId
                });
            }

            await db.SaveChangesAsync(ct);

            // ✅ every code path ends up here
            return acrId;
        }

        private static async Task ApplyEmployeeStateChangesForSpecialTypes(
            AomDbContext db, AcrCreateVm vm, CancellationToken ct)
        {
            switch ((AcrTypeKey)vm.TypeId)
            {
                case AcrTypeKey.Rehire:
                    {
                        var emp = await db.Employees
                            .FirstOrDefaultAsync(e => e.Id == vm.EmployeeId, ct)
                            ?? throw new InvalidOperationException("Employee not found for Rehire ACR.");

                        emp.IsActive = true;   // ✔ REHIRE = ACTIVE
                        break;
                    }

                case AcrTypeKey.Separation:
                    {
                        var emp = await db.Employees
                            .FirstOrDefaultAsync(e => e.Id == vm.EmployeeId, ct)
                            ?? throw new InvalidOperationException("Employee not found for Separation ACR.");

                        emp.IsActive = false;  // ✔ SEPARATION = NOT ACTIVE
                        break;
                    }
            }
        }

        public async Task UpdateAsync(AcrEditVm vm, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var req = await db.Set<AcrRequest>().FirstOrDefaultAsync(r => r.Id == vm.Id, ct)
                      ?? throw new InvalidOperationException($"ACR {vm.Id} not found.");

            int effectiveTypeId =
                vm.TypeId > 0 ? vm.TypeId :
                vm.GetType().GetProperty("TypeKey")?.GetValue(vm) is AcrTypeKey ek ? (int)ek : 0;

            if (effectiveTypeId <= 0) throw new InvalidOperationException("ACR Type is required.");

            var typeExists = await db.Set<AcrType>().AsNoTracking()
                .AnyAsync(t => t.Id == effectiveTypeId, ct);
            if (!typeExists)
                throw new InvalidOperationException($"Invalid ACR TypeId: {effectiveTypeId}.");

            var typeKey = (AcrTypeKey)effectiveTypeId;

            req.EmployeeId = vm.EmployeeId;
            req.AcrTypeId = effectiveTypeId;
            req.EffectiveDate = vm.EffectiveDate ?? req.EffectiveDate;
            req.LastUpdateTime = DateTime.UtcNow;
            req.SubmitterComment = vm.SubmitterComment;

            // ORG
            if (typeKey is AcrTypeKey.OrganizationChange or AcrTypeKey.OrgSchedule)
            {
                var o = await db.Set<AcrOrganization>().FirstOrDefaultAsync(x => x.AcrRequestId == vm.Id, ct);
                if (vm.Organization is null)
                {
                    if (o is not null) db.Remove(o);
                }
                else
                {
                    o ??= db.Add(new AcrOrganization { AcrRequestId = vm.Id }).Entity;
                    o.OrganizationId = vm.Organization.OrganizationId;
                    o.SubOrganizationId = vm.Organization.SubOrganizationId;
                    o.SiteId = vm.Organization.SiteId;
                    o.EmployerId = vm.Organization.EmployerId;
                    o.ManagerId = vm.Organization.ManagerId;
                    o.SupervisorId = vm.Organization.SupervisorId;
                    o.IsLoa = vm.Organization.IsLoa;
                    o.IsIntLoa = vm.Organization.IsIntLoa;
                    o.IsRemote = vm.Organization.IsRemote;
                }
            }

            // SCHEDULE
            if (typeKey is AcrTypeKey.ScheduleChange or AcrTypeKey.OrgSchedule)
            {
                var s = await db.Set<AcrSchedule>().FirstOrDefaultAsync(x => x.AcrRequestId == vm.Id && x.ShiftNumber == 1, ct);
                if (vm.Schedule is null)
                {
                    if (s is not null) db.Remove(s);
                }
                else
                {
                    s ??= db.Add(new AcrSchedule { AcrRequestId = vm.Id, ShiftNumber = 1 }).Entity;
                    s.IsSplitSchedule = vm.Schedule.IsSplitSchedule;
                    s.MondayStart = vm.Schedule.MondayStart; s.MondayEnd = vm.Schedule.MondayEnd;
                    s.TuesdayStart = vm.Schedule.TuesdayStart; s.TuesdayEnd = vm.Schedule.TuesdayEnd;
                    s.WednesdayStart = vm.Schedule.WednesdayStart; s.WednesdayEnd = vm.Schedule.WednesdayEnd;
                    s.ThursdayStart = vm.Schedule.ThursdayStart; s.ThursdayEnd = vm.Schedule.ThursdayEnd;
                    s.FridayStart = vm.Schedule.FridayStart; s.FridayEnd = vm.Schedule.FridayEnd;
                    s.SaturdayStart = vm.Schedule.SaturdayStart; s.SaturdayEnd = vm.Schedule.SaturdayEnd;
                    s.SundayStart = vm.Schedule.SundayStart; s.SundayEnd = vm.Schedule.SundayEnd;
                    s.IsStaticBreakSchedule = vm.Schedule.IsStaticBreakSchedule;

                    bool wantOt = vm.IncludeOvertimeAdjustment || (vm.Schedule?.IsOtAdjustment ?? false);
                    s.IsOtAdjustment = wantOt;
                }
            }

            // OVERTIME upsert / cleanup
            bool wantAnyOt =
                typeKey is AcrTypeKey.OvertimeAdjustment
                || ((typeKey is AcrTypeKey.ScheduleChange or AcrTypeKey.OrgSchedule)
                    && (vm.IncludeOvertimeAdjustment || (vm.Schedule?.IsOtAdjustment ?? false)));

            var existingOt = await db.Set<AcrOvertimeSchedules>().FirstOrDefaultAsync(x => x.AcrRequestId == vm.Id, ct);

            if (wantAnyOt)
            {
                var ot = vm.Overtime ?? new OvertimeAdjustmentDto(null, null, null, null, null, null, null);
                existingOt ??= db.Add(new AcrOvertimeSchedules { AcrRequestId = vm.Id }).Entity;
                existingOt.IsOtAdjustment = true;
                existingOt.MondayTypeId = ot.MondayTypeId;
                existingOt.TuesdayTypeId = ot.TuesdayTypeId;
                existingOt.WednesdayTypeId = ot.WednesdayTypeId;
                existingOt.ThursdayTypeId = ot.ThursdayTypeId;
                existingOt.FridayTypeId = ot.FridayTypeId;
                existingOt.SaturdayTypeId = ot.SaturdayTypeId;
                existingOt.SundayTypeId = ot.SundayTypeId;
            }
            else if (existingOt is not null)
            {
                db.Remove(existingOt);
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task<LastOvertimeAdjustment?> GetLastOvertimeAdjustmentAsync(int employeeId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var row = await (
                from req in db.Set<AcrRequest>().AsNoTracking()
                join ot in db.Set<AcrOvertimeSchedules>().AsNoTracking() on req.Id equals ot.AcrRequestId
                where req.EmployeeId == employeeId
                orderby req.EffectiveDate descending
                select new
                {
                    req.EmployeeId,
                    req.AcrTypeId,
                    req.EffectiveDate,
                    ot.MondayTypeId,
                    ot.TuesdayTypeId,
                    ot.WednesdayTypeId,
                    ot.ThursdayTypeId,
                    ot.FridayTypeId,
                    ot.SaturdayTypeId,
                    ot.SundayTypeId
                }
            ).FirstOrDefaultAsync(ct);

            if (row is null) return null;

            var dto = new OvertimeAdjustmentDto(
                row.MondayTypeId, row.TuesdayTypeId, row.WednesdayTypeId,
                row.ThursdayTypeId, row.FridayTypeId, row.SaturdayTypeId, row.SundayTypeId);

            return new LastOvertimeAdjustment(
                row.EmployeeId, (AcrTypeKey)row.AcrTypeId, row.EffectiveDate, dto);
        }

        public async Task SetStatusAsync(int acrRequestId, int newStatusId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var req = await db.AcrRequests
                .FirstOrDefaultAsync(r => r.Id == acrRequestId, ct)
                ?? throw new InvalidOperationException($"ACR {acrRequestId} not found.");

            var user = CurrentUser;

            req.AcrStatusId = newStatusId;
            req.LastUpdateTime = DateTime.UtcNow;

            switch (newStatusId)
            {
                case ManagerApproved:
                    req.ManagerApprovedBy = user;
                    break;

                case WfmApproved:
                    req.WfmApprovedBy = user;
                    break;

                case Cancelled:
                    req.CancelledBy = user;
                    break;

                case Rejected:
                    req.RejectedBy = user;
                    break;
            }

            await db.SaveChangesAsync(ct);
        }
    }
}