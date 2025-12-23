using System;
using System.Collections.Generic;

namespace MyApplication.Components.Service.Employee.Dtos
{
    public sealed class EmployeeListItem
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleInitial { get; set; }
        public bool IsActive { get; set; }
        public string? Manager { get; set; }
        public string? Supervisor { get; set; }
        public string? Organization { get; set; }
        public string? SubOrganization { get; set; }
        public string? WeekdayProfile { get; set; }
        public string? WeekendProfile { get; set; }
        public string? Schedule { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<DayScheduleDto> DailySchedules { get; set; } = new();

        public string DisplayName
        {
            get
            {
                var name = $"{LastName}, {FirstName}";
                if (!string.IsNullOrWhiteSpace(MiddleInitial)) name += $" {MiddleInitial}";
                return $"{name} ({Id})";
            }
        }
    }

    public class EmployeeFilterOptionsDto
    {
        public List<string> Managers { get; set; } = new();
        public List<string> Supervisors { get; set; } = new();
        public List<string> Organizations { get; set; } = new();
        public List<string> SubOrganizations { get; set; } = new();
    }

    public class DayScheduleDto
    {
        public string Day { get; set; }
        public TimeOnly? Start { get; set; }
        public TimeOnly? End { get; set; }

        public bool IsOff => Start == null || End == null;
        public string DisplayTime => IsOff ? "OFF" : $"{Start:HH:mm}-{End:HH:mm}";
    }

    public sealed class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CorporateId { get; set; }
        public string? DomainLoginName { get; set; }
        public bool IsActive { get; set; }

        public string? Manager { get; set; }
        public string? Supervisor { get; set; }
        public string? Organization { get; set; }
        public string? SubOrganization { get; set; }
        public string? Employer { get; set; }
        public string? Site { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public BreakScheduleDto? Breaks { get; set; }
        public OvertimeScheduleDto? Overtime { get; set; }
    }

    public sealed class BreakScheduleDto
    {
        public int MondayTemplateId { get; set; }
        public int TuesdayTemplateId { get; set; }
        public int WednesdayTemplateId { get; set; }
        public int ThursdayTemplateId { get; set; }
        public int FridayTemplateId { get; set; }
        public int SaturdayTemplateId { get; set; }
        public int SundayTemplateId { get; set; }
    }

    public sealed class OvertimeScheduleDto
    {
        public TimeOnly Duration { get; set; }
        public TimeOnly BeforeShiftMonday { get; set; }
        public TimeOnly AfterShiftMonday { get; set; }
        public TimeOnly BeforeShiftTuesday { get; set; }
        public TimeOnly AfterShiftTuesday { get; set; }
        public TimeOnly BeforeShiftWednesday { get; set; }
        public TimeOnly AfterShiftWednesday { get; set; }
        public TimeOnly BeforeShiftThursday { get; set; }
        public TimeOnly AfterShiftThursday { get; set; }
        public TimeOnly BeforeShiftFriday { get; set; }
        public TimeOnly AfterShiftFriday { get; set; }
        public TimeOnly BeforeShiftSaturday { get; set; }
        public TimeOnly AfterShiftSaturday { get; set; }
        public TimeOnly BeforeShiftSunday { get; set; }
        public TimeOnly AfterShiftSunday { get; set; }
    }

    public class DailyScheduleRow
    {
        public int DetailedScheduleId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool? IsImpacting { get; set; }

        public int ActivityTypeId { get; set; }
        public string? ActivityName { get; set; }
        public int? ActivitySubTypeId { get; set; }

        public string? SubActivityName { get; set; }
        public string? AwsStatusName { get; set; }

        public string SiteName { get; set; }
        public string SiteTimeZoneId { get; set; }

        public string? OrganizationName { get; set; }
        public string? SubOrganizationName { get; set; }
        public string? SupervisorName { get; set; }
    }

    public class EmployeeProfileUpdateDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleInitial { get; set; }
        public string? NmciEmail { get; set; }
        public string? UsnOperatorId { get; set; }
        public string? UsnAdminId { get; set; }
        public string? CorporateEmail { get; set; }
        public string? CorporateId { get; set; }
        public string? DomainLoginName { get; set; }
        public int? AwsId { get; set; }
    }

    public class AwsAgentActivityDto
    {
        public string EventId { get; set; }
        public string AwsId { get; set; }
        public DateTime StartTime { get; set; } // Matches [eventTimeET]
        public string CurrentAgentStatus { get; set; }
        public DateTime EndTime { get; set; }
        public int? Duration { get; set; }
        public Guid AwsGuid { get; set; }
    }

    public class AwsIdentifierLookupDto
    {
        public int Id { get; set; }
        public string? AwsUsername { get; set; }
    }

    public sealed record SchedulesIndexFilter(
        string? EmployeeSearch,
        string? Manager,
        string? Supervisor,
        string? Organization,
        string? SubOrganization
    );
}