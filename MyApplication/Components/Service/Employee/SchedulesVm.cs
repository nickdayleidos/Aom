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
        public string SiteName { get; set; } = string.Empty;
        public string TimeZoneId { get; set; } = "Eastern Standard Time";
        public bool IsExpanded { get; set; } = true;
        public List<EmployeeDailyScheduleVm> Employees { get; set; } = new();
    }

    public class EmployeeDailyScheduleVm
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string OrganizationName { get; set; }
        public string SubOrganizationName { get; set; }
        public string SupervisorName { get; set; }
        public string ShiftString { get; set; }
        public List<ScheduleSegmentVm> Segments { get; set; } = new();     // Lane 1: Plan
        public List<ScheduleSegmentVm> AwsSegments { get; set; } = new();  // Lane 2: Actual
    }
}

public class ScheduleSegmentVm
{
    public int ActivityTypeId { get; set; }
    public int? ActivitySubTypeId { get; set; }
    public string ActivityName { get; set; }
    public string SubActivityName { get; set; }

    public int? AwsStatusId { get; set; }
    public string AwsStatusName { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsImpacting { get; set; }

    public int ZIndex => ActivityName == "Offline" ? 0 : 10;

    // FIXED: Robust matching logic for colors
    public string ColorClass
    {
        get
        {
            var name = SubActivityName != "-" ? SubActivityName : ActivityName;
            if (!string.IsNullOrEmpty(AwsStatusName) && AwsStatusName != "-") name = AwsStatusName;

            if (string.IsNullOrEmpty(name)) return "mud-theme-secondary";

            // Normalize string (Trim and Lowercase for matching)
            var cleanName = name.Trim();

            if (cleanName.Equals("Available", StringComparison.OrdinalIgnoreCase)) return "mud-theme-success";
            if (cleanName.Equals("Offline", StringComparison.OrdinalIgnoreCase)) return "mud-theme-dark";
            if (cleanName.Equals("Lunch", StringComparison.OrdinalIgnoreCase)) return "mud-theme-info";
            if (cleanName.Equals("Break", StringComparison.OrdinalIgnoreCase)) return "mud-theme-warning";
            if (cleanName.Equals("Training", StringComparison.OrdinalIgnoreCase)) return "mud-theme-primary";
            if (cleanName.Equals("Meeting", StringComparison.OrdinalIgnoreCase)) return "mud-theme-primary";
            if (cleanName.Equals("Coaching", StringComparison.OrdinalIgnoreCase)) return "mud-theme-primary";
            if (cleanName.Equals("Peer Mentoring", StringComparison.OrdinalIgnoreCase)) return "mud-theme-primary";
            if (cleanName.Equals("Admin Tasks", StringComparison.OrdinalIgnoreCase)) return "mud-theme-secondary";
            if (cleanName.Equals("OL Customer Work", StringComparison.OrdinalIgnoreCase)) return "mud-theme-info";
            if (cleanName.Equals("System", StringComparison.OrdinalIgnoreCase)) return "mud-theme-dark";

            return "mud-theme-secondary"; // Default Pink if unknown
        }
    }
}
