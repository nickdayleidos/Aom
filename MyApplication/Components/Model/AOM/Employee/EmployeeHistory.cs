using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("EmployeeHistory", Schema = "Employee")]
    public class EmployeeHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }

        public int SupervisorId { get; set; }          // FK -> Supervisor.Id
        public Supervisor? Supervisor { get; set; }

        public int ManagerId { get; set; }             // FK -> Manager.Id
        public Manager? Manager { get; set; }

        public int SiteId { get; set; }
        public Site? Site { get; set; }

        public int EmployerId { get; set; }
        public Employer? Employer { get; set; }

        public int? OrganizationId { get; set; }       // nullable FK
        public Organization? Organization { get; set; }

        public int SubOrganizationId { get; set; }
        public SubOrganization? SubOrganization { get; set; }

        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsLoa { get; set; }
        public bool IsIntLoa { get; set; }
        public bool IsRemote { get; set; }

        public int ScheduleId { get; set; }            // FK -> Schedule.Id
        public AcrSchedule? Schedule { get; set; }
        
      

    }
}
