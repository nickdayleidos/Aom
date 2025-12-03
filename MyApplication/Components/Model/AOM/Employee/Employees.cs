using MyApplication.Components.Model.AOM.Aws;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Employees", Schema = "Employee")]
    public class Employees
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, Column(TypeName = "varchar(64)")]
        public string? FirstName { get; set; }

        [Required, Column(TypeName = "varchar(64)")]
        public string? LastName { get; set; }

        [Column(TypeName = "varchar(1)")]
        public string? MiddleInitial { get; set; }

        [Column(TypeName = "varchar(64)")] public string? NmciEmail { get; set; }
        [Column(TypeName = "varchar(64)")] public string? UsnOperatorId { get; set; }
        [Column(TypeName = "varchar(64)")] public string? UsnAdminId { get; set; }
        [Column(TypeName = "varchar(64)")] public string? CorporateEmail { get; set; }
        [Column(TypeName = "varchar(32)")] public string? CorporateId { get; set; }
        [Column(TypeName = "varchar(32)")] public string? DomainLoginName { get; set; }
        public int? AwsId { get; set; }
        public Identifiers? Aws { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class EmployeeHistory
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }

        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }
        public int? ManagerId { get; set; }
        public Manager? Manager { get; set; }
        public int? SiteId { get; set; }
        public Site? Site { get; set; }
        public int? EmployerId { get; set; }
        public Employer? Employer { get; set; }
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public int? SubOrganizationId { get; set; }
        public SubOrganization? SubOrganization { get; set; }

        public DateTime EffectiveDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLoa { get; set; }
        public bool? IsIntLoa { get; set; }
        public bool? IsRemote { get; set; }
        // 👇 point to the ACR that carries the schedules for this snapshot
        public int? ScheduleRequestId { get; set; }
        public AcrRequest? ScheduleRequest { get; set; }
        public int? OvertimeRequestId { get; set; }
        public AcrRequest? OvertimeRequest { get; set; }
        public int? SourceAcrId { get; set; } // provenance
    }

}
