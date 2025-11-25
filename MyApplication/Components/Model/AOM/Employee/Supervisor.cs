using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Supervisor", Schema = "Employee")]
    public class Supervisor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmployeeId { get; set; }   // FK -> Employees.Id
        public bool? IsActive { get; set; } = true;

        // 🚫 Removed self-collection
    }
}
