using System.ComponentModel.DataAnnotations.Schema;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Model.AOM.Aws
{
    [Table("EmployeeRoutingProfile", Schema = "Aws")]
    public class EmployeeRoutingProfile
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }
        public int? WeekdayProfileId { get; set; }
        public RoutingProfile? WeekdayProfile { get; set; }
        public int? WeekendProfileId { get; set; }
        public RoutingProfile? WeekendProfile { get; set; }
    }
}
