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
    }

    public class ScheduleSegmentVm
    {
        public int ActivityTypeId { get; set; }
        public int ActivitySubTypeId { get; set; } // Needed for priority logic
        public string ActivityName { get; set; }
        public string SubActivityName { get; set; }
        public int? AwsStatusId { get; set; }      // Needed for coloring
        public string AwsStatusName { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public bool IsImpacting { get; set; }

        // --- Presentation Logic ---

        public string ColorClass => AwsStatusId switch
        {
            1 => "mud-theme-dark",       // Offline
            2 => "mud-theme-success",    // Available
            3 => "mud-theme-info",       // OL Customer Work
            4 => "mud-theme-secondary",  // Admin Tasks
            5 => "mud-theme-warning",    // Break
            6 => "mud-theme-primary",    // Coaching
            7 => "mud-theme-info",       // Lunch
            8 => "mud-theme-primary",    // Meeting
            9 => "mud-theme-primary",    // Peer Mentoring
            10 => "mud-theme-dark",      // System
            11 => "mud-theme-primary",   // Training
            12 => "mud-theme-secondary", // Corporate Engagement
            _ => "mud-theme-dark"        // Default
        };

        // Layering Priority: Breaks (Status 5), Lunches (Status 7) & specific Activity Subtypes on top.
        public int ZIndex =>
            (AwsStatusId == 5 || AwsStatusId == 7 ||
             ActivitySubTypeId == 47 || ActivitySubTypeId == 49 ||
             ActivitySubTypeId == 53 || ActivitySubTypeId == 54)
            ? 10 : 1;
    }
}