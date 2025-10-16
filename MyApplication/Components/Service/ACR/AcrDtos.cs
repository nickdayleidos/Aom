// MyApplication/Components/Service/Acr/AcrDtos.cs

// NOTE:
// - Create endpoints should infer ACR Type from the endpoint called
//   (e.g., CreateOrganizationChangeAsync => "Organization Change").
// - Server should set initial status to Submitted (1). These DTOs
//   do not carry StatusId or TypeId.



public record OrganizationChangeDto(
    int EmployeeId,
    DateOnly EffectiveDate,
    string? SubmitterComment,
    int? ManagerId,
    int? SupervisorId,
    int? OrganizationId,
    int? SubOrganizationId,
    int? EmployerId,
    int? SiteId,
    bool? IsActive,
    bool? IsLoa,
    bool? IsIntLoa,
    bool? IsRemote
);

// AcrDtos.cs
public record ScheduleChangeDto(
    int EmployeeId,
    DateOnly EffectiveDate,
    string? SubmitterComment,
    bool IsSplitSchedule,                  // toggle controls whether segment 2 is used
                                           // Segment 1 (required if provided)
    TimeOnly? MondayStart, TimeOnly? MondayEnd,
    TimeOnly? TuesdayStart, TimeOnly? TuesdayEnd,
    TimeOnly? WednesdayStart, TimeOnly? WednesdayEnd,
    TimeOnly? ThursdayStart, TimeOnly? ThursdayEnd,
    TimeOnly? FridayStart, TimeOnly? FridayEnd,
    TimeOnly? SaturdayStart, TimeOnly? SaturdayEnd,
    TimeOnly? SundayStart, TimeOnly? SundayEnd,
    // Segment 2 (used only when IsSplitSchedule == true)
    TimeOnly? MondayStart2, TimeOnly? MondayEnd2,
    TimeOnly? TuesdayStart2, TimeOnly? TuesdayEnd2,
    TimeOnly? WednesdayStart2, TimeOnly? WednesdayEnd2,
    TimeOnly? ThursdayStart2, TimeOnly? ThursdayEnd2,
    TimeOnly? FridayStart2, TimeOnly? FridayEnd2,
    TimeOnly? SaturdayStart2, TimeOnly? SaturdayEnd2,
    TimeOnly? SundayStart2, TimeOnly? SundayEnd2,
    // Meta (kept)
    int? BreakTime, int? Breaks, int? LunchTime
);

public record OrgAndScheduleChangeDto(OrganizationChangeDto Org, ScheduleChangeDto Sch);

public record NewHireDto(
    string FirstName,
    string LastName,
    string? MiddleInitial,
    string? NmciEmail,
    string? UsnOperatorId,
    string? UsnAdminId,
    string? FlankspeedEmail,
    string? CorporateEmail,
    string? CorporateId,
    string? DomainLoginName,
    DateOnly EffectiveDate,
    string? SubmitterComment
);

public record SeparationDto(
    int EmployeeId,
    DateOnly EffectiveDate,
    string? SubmitterComment
);

public sealed record RehireDto(
    int EmployeeId,
    DateOnly EffectiveDate,
    string? SubmitterComment
);
