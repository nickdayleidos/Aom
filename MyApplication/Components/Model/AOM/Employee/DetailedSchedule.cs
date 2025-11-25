using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("DetailedSchedule", Schema = "Employee")]
    public class DetailedSchedule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employees { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ActivityTypeId { get; set; }
        public ActivityType? ActivityType { get; set; }
        public int ActivitySubTypeId { get; set; }
        public ActivitySubType? ActivitySubType { get; set; }
        public int? AwsStatusId { get; set; }
        public int? Minutes { get; set; }
        public bool? IsImpacting { get; set; }
    }
}
