using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Manager", Schema = "Employee")]
    public class Manager
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmployeeId { get; set; }   // FK -> Employees.Id
        public bool? IsActive { get; set; } = true;

        // 🚫 Removed self-collection
    }
}
