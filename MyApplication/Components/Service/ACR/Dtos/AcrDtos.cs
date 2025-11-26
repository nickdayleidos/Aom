using MyApplication.Components.Service.Acr.MyApplication.Components.Service.Acr;
using System;

namespace MyApplication.Components.Service.Acr
{
    public enum AcrTypeKey
    {
        NewHire = 1,
        OrganizationChange = 2,
        ScheduleChange = 3,
        OrgSchedule = 4,
        Separation = 5,
        Rehire = 6,
        OvertimeAdjustment = 7
    }

    namespace MyApplication.Components.Service.Acr
    {
        public sealed record NewHireDto(
            string? FirstName,
            string? LastName,
            string? MiddleInitial,
            string? NmciEmail,
            string? UsnOperatorId,
            string? UsnAdminId,
            string? CorporateEmail,
            string? CorporateId,
            string? DomainLoginName
        );
    }

    public sealed record AcrCreateVm
    {
        public int EmployeeId { get; init; }
        public int TypeId { get; init; }
        public DateOnly? EffectiveDate { get; init; }
        public string? SubmitterComment { get; init; }

        public OrganizationChangeDto? Organization { get; init; }
        public ScheduleChangeDto? Schedule { get; init; }
        public OvertimeAdjustmentDto? Overtime { get; init; }
        public bool IncludeOvertimeAdjustment { get; init; }

        // 👇 New
        public NewHireDto? NewHire { get; init; }
    }

    public sealed record AcrDetailsVm(
    int AcrRequestId,
    AcrTypeKey TypeKey,
    int EmployeeId,
    string EmployeeName,
    DateOnly EffectiveDate,
    string SubmitterComment,     // 🆕 add here
    OrganizationChangeDto? Organization,
    ScheduleChangeDto? Schedule,
    OvertimeAdjustmentDto? Overtime
);

    public sealed record AcrEditVm
    {
        // Request identity
        public int Id { get; init; }

        // Required
        public int EmployeeId { get; init; }
        public int TypeId { get; init; }   // <— used by service for AcrType FK

        // Back-compat for any code that still reads enum TypeKey
        public AcrTypeKey TypeKey => (AcrTypeKey)TypeId;

        // Optional
        public DateOnly? EffectiveDate { get; init; }
        public string? SubmitterComment { get; init; }

        // Sections
        public OrganizationChangeDto? Organization { get; init; }
        public ScheduleChangeDto? Schedule { get; init; }

        // Overtime
        public bool IncludeOvertimeAdjustment { get; init; }
        public OvertimeAdjustmentDto? Overtime { get; init; }
    }

    public sealed record OrganizationChangeDto(
        int? OrganizationId, int? SubOrganizationId, int? SiteId, int? EmployerId,
        int? ManagerId, int? SupervisorId, bool? IsLoa, bool? IsIntLoa, bool? IsRemote);

    public sealed record ScheduleChangeDto(
        bool? IsSplitSchedule, int? ShiftNumber,
        TimeOnly? MondayStart, TimeOnly? MondayEnd,
        TimeOnly? TuesdayStart, TimeOnly? TuesdayEnd,
        TimeOnly? WednesdayStart, TimeOnly? WednesdayEnd,
        TimeOnly? ThursdayStart, TimeOnly? ThursdayEnd,
        TimeOnly? FridayStart, TimeOnly? FridayEnd,
        TimeOnly? SaturdayStart, TimeOnly? SaturdayEnd,
        TimeOnly? SundayStart, TimeOnly? SundayEnd,
        bool? IsStaticBreakSchedule,
        bool? IsOtAdjustment);

    public sealed record OvertimeAdjustmentDto(
        int? MondayTypeId, int? TuesdayTypeId, int? WednesdayTypeId,
        int? ThursdayTypeId, int? FridayTypeId, int? SaturdayTypeId, int? SundayTypeId);

    // NOTE: DO NOT declare LastOvertimeAdjustment here — it already exists elsewhere in this namespace.
}