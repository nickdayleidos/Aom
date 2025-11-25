// Forms/Defaults.cs (or at the top of each form file for convenience)
using System;
using System.Collections.Generic;
using MyApplication.Components.Model.AOM.Employee;

public sealed record OrgDefaults(
    Employees? Manager,
    Employees? Supervisor,
    Organization? Organization,
    SubOrganization? SubOrganization,
    Employer? Employer,
    Site? Site,
    bool? IsActive,
    bool? IsLoa,
    bool? IsIntLoa,
    bool? IsRemote

);

public sealed record SchDay(TimeSpan? Start, TimeSpan? End);
public sealed record SchDefaults(
    Dictionary<string, SchDay> Seg1,
    Dictionary<string, SchDay>? Seg2,
    int? BreakTime,
    int? Breaks,
    int? LunchTime,
    bool IsSplit
);
