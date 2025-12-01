using System;
using System.Collections.Generic;

namespace MyApplication.Components.Service.Employee
{
    public class ScheduleDashboardVm
    {
        public DateOnly Date { get; set; }
        public List<SiteScheduleGroupVm> Sites { get; set; } = new();
    }

    public class SiteScheduleGroupVm
    {
        public string SiteName { get; set; }
        public string TimeZoneId { get; set; }
        public bool IsExpanded { get; set; }
        public List<EmployeeDailyScheduleVm> Employees { get; set; } = new();
    }

    public class EmployeeDailyScheduleVm
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Title { get; set; }
        public string ShiftString { get; set; }

        // NEW: Organization & Supervisor Info
        public string OrganizationName { get; set; }
        public string SubOrganizationName { get; set; }
        public string SupervisorName { get; set; }

        public List<ScheduleSegmentVm> Segments { get; set; } = new();
    }

    public class ScheduleSegmentVm
    {
        public int ActivityTypeId { get; set; }
        public string ActivityName { get; set; }
        public string SubActivityName { get; set; }
        public string AwsStatusName { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public bool IsImpacting { get; set; }
    }
}