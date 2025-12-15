using MyApplication.Components.Model.AOM.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApplication.Components.Service.Acr
{
    // ==========================================
    // Enums
    // ==========================================
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

    // ==========================================
    // View Models (For Forms/Pages)
    // ==========================================

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

        public NewHireDto? NewHire { get; init; }
    }

    public sealed record AcrEditVm
    {
        public int Id { get; init; }
        public int EmployeeId { get; init; }
        public int TypeId { get; init; }
        public AcrTypeKey TypeKey => (AcrTypeKey)TypeId;

        public DateOnly? EffectiveDate { get; init; }
        public string? SubmitterComment { get; init; }

        public OrganizationChangeDto? Organization { get; init; }
        public ScheduleChangeDto? Schedule { get; init; }
        public bool IncludeOvertimeAdjustment { get; init; }
        public OvertimeAdjustmentDto? Overtime { get; init; }
    }

    public sealed record AcrDetailsVm(
        int AcrRequestId,
        AcrTypeKey TypeKey,
        int EmployeeId,
        string EmployeeName,
        DateOnly EffectiveDate,
        string SubmitterComment,
        OrganizationChangeDto? Organization,
        ScheduleChangeDto? Schedule,
        OvertimeAdjustmentDto? Overtime
    );

    // ==========================================
    // Data Transfer Objects (Nested Data)
    // ==========================================

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

    public sealed record OrganizationChangeDto(
        int? OrganizationId, int? SubOrganizationId, int? SiteId, int? EmployerId,
        int? ManagerId, int? SupervisorId, bool? IsLoa, bool? IsIntLoa, bool? IsRemote);

    // IMPORTANT: Mutable record to support both UI cloning and Service updates
    public record ScheduleChangeDto
    {
        public int ShiftNumber { get; set; } = 1;
        public bool? IsSplitSchedule { get; set; } = false;
        public bool? IsStaticBreakSchedule { get; set; } = false;
        public bool? IsOtAdjustment { get; set; } = false;

        public TimeOnly? MondayStart { get; set; }
        public TimeOnly? MondayEnd { get; set; }
        public TimeOnly? TuesdayStart { get; set; }
        public TimeOnly? TuesdayEnd { get; set; }
        public TimeOnly? WednesdayStart { get; set; }
        public TimeOnly? WednesdayEnd { get; set; }
        public TimeOnly? ThursdayStart { get; set; }
        public TimeOnly? ThursdayEnd { get; set; }
        public TimeOnly? FridayStart { get; set; }
        public TimeOnly? FridayEnd { get; set; }
        public TimeOnly? SaturdayStart { get; set; }
        public TimeOnly? SaturdayEnd { get; set; }
        public TimeOnly? SundayStart { get; set; }
        public TimeOnly? SundayEnd { get; set; }
    }

    public sealed record OvertimeAdjustmentDto(
        int? MondayTypeId, int? TuesdayTypeId, int? WednesdayTypeId,
        int? ThursdayTypeId, int? FridayTypeId, int? SaturdayTypeId, int? SundayTypeId);

    public sealed record LastOvertimeAdjustment(
        int EmployeeId,
        AcrTypeKey Type,
        DateOnly EffectiveDate,
        OvertimeAdjustmentDto Adjustment
    );

    // ==========================================
    // History & Previous Details (Used in Left Panel)
    // ==========================================

    // THIS WAS MISSING
    public sealed class PrevDetailsVm
    {
        public int? EmployeeId { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public string? SubmitterComment { get; set; }

        public int? EmployerId { get; set; }
        public int? SiteId { get; set; }
        public int? OrganizationId { get; set; }
        public int? SubOrganizationId { get; set; }
        public int? ManagerId { get; set; }
        public int? SupervisorId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLoa { get; set; }
        public bool? IsIntLoa { get; set; }
        public bool? IsRemote { get; set; }

        public string? EmployerName { get; set; }
        public string? SiteCode { get; set; }
        public string? OrganizationName { get; set; }
        public string? SubOrganizationName { get; set; }
        public string? ManagerName { get; set; }
        public string? SupervisorName { get; set; }

        public string? SchMon { get; set; }
        public string? SchTue { get; set; }
        public string? SchWed { get; set; }
        public string? SchThu { get; set; }
        public string? SchFri { get; set; }
        public string? SchSat { get; set; }
        public string? SchSun { get; set; }

        public int? ShiftNumber { get; set; }
        public bool? IsStaticBreakSchedule { get; set; }
        public string? OTMon { get; set; }
        public string? OTTue { get; set; }
        public string? OTWed { get; set; }
        public string? OTThu { get; set; }
        public string? OTFri { get; set; }
        public string? OTSat { get; set; }
        public string? OTSun { get; set; }

        public PrevDetailsVm() { }
    }

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

    // ==========================================
    // Lists & Filters
    // ==========================================

    public sealed record AcrRequestListItem(
        int Id,
        int EmployeeId,
        string EmployeeName,
        string AcrType,
        string? AcrStatus,
        DateOnly EffectiveDate,
        DateTime? SubmitTime
    );

    public sealed record AcrIndexFilter(
     int? AcrTypeId,
     int? AcrStatusId,
     string? EmployeeSearch,
     int? ManagerId,        
     int? SupervisorId,     
     DateOnly? EffectiveFrom,
     DateOnly? EffectiveTo,
     bool ActiveOnly
 );

    public sealed record PagedResult<T>(List<T> Items, int TotalCount);
}