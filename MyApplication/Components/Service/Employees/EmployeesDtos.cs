using System;

namespace MyApplication.Components.Service.Employee.Dtos
{
    public sealed class EmployeeListItem
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? Manager { get; set; }
        public string? Supervisor { get; set; }
        public string? Organization { get; set; }
        public string? SubOrganization { get; set; }
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
}
