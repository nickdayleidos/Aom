using System;
using System.Collections.Generic;

namespace MyApplication.Components.Service.Employee
{
    public class EmployeeFullDetailsVm
    {
        public int EmployeeId { get; set; }

        // --- Profile ---
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MiddleInitial { get; set; } = null!;
        public string NmciEmail { get; set; } = null!;
        public string UsnOperatorId { get; set; } = null!;
        public string UsnAdminId { get; set; } = null!;
        public string CorporateEmail { get; set; } = null!;
        public string CorporateId { get; set; } = null!;
        public string DomainLoginName { get; set; } = null!;
        public int? AwsId { get; set; }
        public string? AwsUsername { get; set; }

        // --- Assignment ---
        public bool HasHistory { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public string Employer { get; set; } = null!;
        public string Site { get; set; } = null!;
        public string SiteTimeZoneId { get; set; } = null!; // NEW: For dynamic time conversion
        public string Organization { get; set; } = null!;
        public string SubOrganization { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public string Supervisor { get; set; } = null!;
        public bool IsRemote { get; set; }
        public bool IsLoa { get; set; }
        public bool IsIntLoa { get; set; }

        // --- Schedules ---
        public ScheduleDetailsVm Schedule { get; set; } = null!;
        public ScheduleDetailsVm Schedule2 { get; set; } = null!;

        // --- Other Details ---
        public OvertimeDetailsVm Overtime { get; set; } = null!;
        public StaticBreakDetailsVm StaticBreaks { get; set; } = null!;

        // --- Lists ---
        public List<string> Skills { get; set; } = new();
        public List<AcrHistoryDto> AcrHistory { get; set; } = new();
        public List<OperaHistoryDto> OperaHistory { get; set; } = new();
    }

    public class ScheduleDetailsVm
    {
        public bool IsSplit { get; set; }

        // Formatted strings (Default/ET)
        public string Mon { get; set; } = null!;
        public string Tue { get; set; } = null!;
        public string Wed { get; set; } = null!;
        public string Thu { get; set; } = null!;
        public string Fri { get; set; } = null!;
        public string Sat { get; set; } = null!;
        public string Sun { get; set; } = null!;

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
        public string Mon { get; set; } = null!;
        public string Tue { get; set; } = null!;
        public string Wed { get; set; } = null!;
        public string Thu { get; set; } = null!;
        public string Fri { get; set; } = null!;
        public string Sat { get; set; } = null!;
        public string Sun { get; set; } = null!;
    }

    public class StaticBreakDetailsVm
    {
        public string Mon { get; set; } = null!;
        public string Tue { get; set; } = null!;
        public string Wed { get; set; } = null!;
        public string Thu { get; set; } = null!;
        public string Fri { get; set; } = null!;
        public string Sat { get; set; } = null!;
        public string Sun { get; set; } = null!;
    }

    public class AcrHistoryDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateOnly EffectiveDate { get; set; }
        public DateTime? Submitted { get; set; }
    }

    public class OperaHistoryDto
    {
        public int RequestId { get; set; }
        public string Type { get; set; } = null!;
        public string SubType { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = null!;
    }
}