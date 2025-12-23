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
        public List<ScheduleSegmentVm> Segments { get; set; } = new();
        public List<ScheduleSegmentVm> AlertSegments { get; set; } = new();
        public List<ScheduleSegmentVm> AwsSegments { get; set; } = new();
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

    public string ColorClass
    {
        get
        {
            var name = SubActivityName != "-" ? SubActivityName : ActivityName;
            if (!string.IsNullOrEmpty(AwsStatusName) && AwsStatusName != "-") name = AwsStatusName;

            if (string.IsNullOrEmpty(name)) return "activity-default";

            var cleanName = name.Trim();

            if (cleanName.Equals("Offline", StringComparison.OrdinalIgnoreCase)) return "activity-offline";
            if (cleanName.Equals("Admin Tasks", StringComparison.OrdinalIgnoreCase)) return "activity-admin";
            if (cleanName.Equals("System", StringComparison.OrdinalIgnoreCase)) return "activity-system";
            if (cleanName.Equals("Available", StringComparison.OrdinalIgnoreCase)) return "activity-available";
            if (cleanName.Equals("OL Customer Work", StringComparison.OrdinalIgnoreCase)) return "activity-ol-customer";
            if (cleanName.Equals("Peer Mentoring", StringComparison.OrdinalIgnoreCase)) return "activity-peer";
            if (cleanName.Equals("Break", StringComparison.OrdinalIgnoreCase)) return "activity-break";
            if (cleanName.Equals("Lunch", StringComparison.OrdinalIgnoreCase)) return "activity-lunch";
            if (cleanName.Equals("Corporate Engagement", StringComparison.OrdinalIgnoreCase) || cleanName.Equals("Corp Engagement", StringComparison.OrdinalIgnoreCase)) return "activity-corporate";
            if (cleanName.Equals("Coaching", StringComparison.OrdinalIgnoreCase)) return "activity-coaching";
            if (cleanName.Equals("Meeting", StringComparison.OrdinalIgnoreCase)) return "activity-meeting";
            if (cleanName.Equals("Training", StringComparison.OrdinalIgnoreCase)) return "activity-training";

            return "activity-default";
        }
    }
}