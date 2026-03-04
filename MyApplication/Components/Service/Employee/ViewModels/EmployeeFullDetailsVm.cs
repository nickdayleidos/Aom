using System;
using System.Collections.Generic;

namespace MyApplication.Components.Service.Employee
{
    public class EmployeeFullDetailsVm
    {
        public int EmployeeId { get; set; }

        // --- Profile ---
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string NmciEmail { get; set; }
        public string UsnOperatorId { get; set; }
        public string UsnAdminId { get; set; }
        public string CorporateEmail { get; set; }
        public string CorporateId { get; set; }
        public string DomainLoginName { get; set; }
        public int? AwsId { get; set; }
        public string? AwsUsername { get; set; }

        // --- Assignment ---
        public bool HasHistory { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public string Employer { get; set; }
        public string Site { get; set; }
        public string SiteTimeZoneId { get; set; } // NEW: For dynamic time conversion
        public string Organization { get; set; }
        public string SubOrganization { get; set; }
        public string Manager { get; set; }
        public string Supervisor { get; set; }
        public bool IsRemote { get; set; }
        public bool IsLoa { get; set; }
        public bool IsIntLoa { get; set; }

        // --- Schedules ---
        public ScheduleDetailsVm Schedule { get; set; }
        public ScheduleDetailsVm Schedule2 { get; set; }

        // --- Other Details ---
        public OvertimeDetailsVm Overtime { get; set; }
        public StaticBreakDetailsVm StaticBreaks { get; set; }

        // --- Lists ---
        public List<string> Skills { get; set; } = new();
        public List<AcrHistoryDto> AcrHistory { get; set; } = new();
        public List<OperaHistoryDto> OperaHistory { get; set; } = new();
    }

    public class ScheduleDetailsVm
    {
        public bool IsSplit { get; set; }

        // Formatted strings (Default/ET)
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }

        // NEW: Raw Times for Dynamic Conversion
        public TimeOnly? MonStart { get; set; }
        public TimeOnly? MonEnd { get; set; }
        public TimeOnly? TueStart { get; set; }
        public TimeOnly? TueEnd { get; set; }
        public TimeOnly? WedStart { get; set; }
        public TimeOnly? WedEnd { get; set; }
        public TimeOnly? ThuStart { get; set; }
        public TimeOnly? ThuEnd { get; set; }
        public TimeOnly? FriStart { get; set; }
        public TimeOnly? FriEnd { get; set; }
        public TimeOnly? SatStart { get; set; }
        public TimeOnly? SatEnd { get; set; }
        public TimeOnly? SunStart { get; set; }
        public TimeOnly? SunEnd { get; set; }
    }

    public class OvertimeDetailsVm
    {
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
    }

    public class StaticBreakDetailsVm
    {
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
    }

    public class AcrHistoryDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateTime? Submitted { get; set; }
    }

    public class OperaHistoryDto
    {
        public int RequestId { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
    }
}