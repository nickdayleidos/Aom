using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("AcrType", Schema = "Employee")]
    public class AcrType
    {
        // Key by convention; fluent config will set HasKey in DbContext
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Table("AcrStatus", Schema = "Employee")]
    public class AcrStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Table("AcrRequest", Schema = "Employee")]
    public class AcrRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }        // ✅ rename to Employee (not Employees)

        public int? AcrTypeId { get; set; }
        public AcrType? AcrType { get; set; }
        public int AcrStatusId { get; set; }
        public AcrStatus? AcrStatus { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? SubmittedBy { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? RejectedBy { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? CancelledBy { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? ManagerApprovedBy { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? WfmApprovedBy { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string? SubmitterComment { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string? WfmComment { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? LastUpdateTime { get; set; }
    }

    [Table("AcrOrganization", Schema = "Employee")]
    public class AcrOrganization
    {
        public int Id { get; set; }

        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }

        public int? ManagerId { get; set; }
        public Manager? Manager { get; set; }

        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }

        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public int? SubOrganizationId { get; set; }
        public SubOrganization? SubOrganization { get; set; }

        public int? EmployerId { get; set; }
        public Employer? Employer { get; set; }
        public int? SiteId { get; set; }
        public Site? Site { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLoa { get; set; }
        public bool? IsIntLoa { get; set; }
        public bool? IsRemote { get; set; }
    }

    [Table("AcrSchedule", Schema = "Employee")]
    public class AcrSchedule
    {
        public int Id { get; set; }
        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }
        public bool? IsSplitSchedule { get; set; }
        // You updated to int: 1 = first block, 2 = second block
        public int? ShiftNumber { get; set; }
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
        public int? BreakTime { get; set; }
        public int? Breaks { get; set; }
        public int? LunchTime { get; set; }
        public int breakTemplateId { get; set; }

    }
}