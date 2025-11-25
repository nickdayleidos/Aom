using System;


namespace MyApplication.Components.Service.Acr;


public sealed record EmployeeHistorySnapshot(
int EmployeeId,
string EmployeeDisplay,
int? OrganizationId,
string? OrganizationName,
int? SubOrganizationId,
string? SubOrganizationName,
int? SiteId,
string? SiteName,
string? CurrentScheduleLabel,
TimeOnly? PrevStart,
TimeOnly? PrevEnd,
bool? PrevOvertimeEligible,
decimal? PrevOvertimeAdjustment,
int? ScheduleRequestId,
int? OvertimeRequestId,
bool? IsLoa,
bool? IsIntLoa,
bool? IsRemote
);