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
        [Column(TypeName = "varchar(64)")] public string? FlankspeedEmail { get; set; }
        [Column(TypeName = "varchar(64)")] public string? CorporateEmail { get; set; }
        [Column(TypeName = "varchar(32)")] public string? CorporateId { get; set; }
        [Column(TypeName = "varchar(32)")] public string? DomainLoginName { get; set; }

        public bool IsActive { get; set; } = true;

        // 🚫 Removed: public ICollection<Employees> EmployeeLookup ...
    }
}
