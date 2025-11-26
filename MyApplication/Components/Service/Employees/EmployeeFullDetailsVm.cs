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

        // --- Assignment ---
        public bool HasHistory { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public string Employer { get; set; }
        public string Site { get; set; }
        public string Organization { get; set; }
        public string SubOrganization { get; set; }
        public string Manager { get; set; }
        public string Supervisor { get; set; }
        public bool IsRemote { get; set; }
        public bool IsLoa { get; set; }
        public bool IsIntLoa { get; set; }

        // --- Schedules (Now supports Split) ---
        public ScheduleDetailsVm Schedule { get; set; }
        public ScheduleDetailsVm Schedule2 { get; set; } // New: For Shift 2

        // --- Other Details ---
        public OvertimeDetailsVm Overtime { get; set; }
        public StaticBreakDetailsVm StaticBreaks { get; set; }

        // --- NEW SECTIONS ---
        public List<string> Skills { get; set; } = new();
        public List<AcrHistoryDto> AcrHistory { get; set; } = new();
        public List<OperaHistoryDto> OperaHistory { get; set; } = new();
    }

    public class ScheduleDetailsVm
    {
        public bool IsSplit { get; set; }
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
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