using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("AcrType", Schema = "Employee")]
    public class AcrType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
    }

    [Table("AcrStatus", Schema = "Employee")]
    public class AcrStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
    }

    [Table("AcrRequest", Schema = "Employee")]
    public class AcrRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }

        public int? AcrTypeId { get; set; }
        public AcrType? AcrType { get; set; }

        public int AcrStatusId { get; set; }
        public AcrStatus? AcrStatus { get; set; }

        public bool? IsActive { get; set; } = true;

        [Column(TypeName = "varchar(32)")] public string? SubmittedBy { get; set; }
        [Column(TypeName = "varchar(32)")] public string? RejectedBy { get; set; }
        [Column(TypeName = "varchar(32)")] public string? CancelledBy { get; set; }
        [Column(TypeName = "varchar(32)")] public string? ManagerApprovedBy { get; set; }
        [Column(TypeName = "varchar(32)")] public string? WfmApprovedBy { get; set; }
        [Column(TypeName = "varchar(128)")] public string? SubmitterComment { get; set; }
        [Column(TypeName = "varchar(128)")] public string? WfmComment { get; set; }

        public DateOnly EffectiveDate { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? LastUpdateTime { get; set; }
        public DateTime? ProcessedTime { get; set; }
        public DateTime? RevertedTime { get; set; }

        // ... inside AcrRequest class

        // 1-to-1: One request has one Organization change
        public virtual AcrOrganization? AcrOrganization { get; set; }

        // 1-to-1: One request has one Overtime schedule
        public virtual AcrOvertimeSchedules? AcrOvertimeSchedule { get; set; }

        // 1-to-Many: One request can have 2 shifts (Split Schedule)
        public virtual ICollection<AcrSchedule> AcrSchedules { get; set; } = new List<AcrSchedule>();
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

        public bool? IsLoa { get; set; } = false;
        public bool? IsIntLoa { get; set; } = false;
        public bool? IsRemote { get; set; } = false;
    }

    [Table("AcrSchedule", Schema = "Employee")]
    public class AcrSchedule
    {
        public int Id { get; set; }

        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }

        public bool? IsSplitSchedule { get; set; }
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
        public bool? IsStaticBreakSchedule { get; set; }
        public bool? IsOtAdjustment { get; set; } = false;
    }

    [Table("AcrOvertimeSchedules", Schema = "Employee")]
    public class AcrOvertimeSchedules
    {
        public int Id { get; set; }

        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }

        public bool IsOtAdjustment { get; set; } = false;

        public int? MondayTypeId { get; set; }
        public int? TuesdayTypeId { get; set; }
        public int? WednesdayTypeId { get; set; }
        public int? ThursdayTypeId { get; set; }
        public int? FridayTypeId { get; set; }
        public int? SaturdayTypeId { get; set; }
        public int? SundayTypeId { get; set; }

        // NEW: optional navs (so EF can build 7 FKs to OvertimeTypes)
        public AcrOvertimeTypes? MondayType { get; set; }
        public AcrOvertimeTypes? TuesdayType { get; set; }
        public AcrOvertimeTypes? WednesdayType { get; set; }
        public AcrOvertimeTypes? ThursdayType { get; set; }
        public AcrOvertimeTypes? FridayType { get; set; }
        public AcrOvertimeTypes? SaturdayType { get; set; }
        public AcrOvertimeTypes? SundayType { get; set; }
    }

    [Table("OvertimeTypes", Schema = "Employee")]
    public class AcrOvertimeTypes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
    }


}
