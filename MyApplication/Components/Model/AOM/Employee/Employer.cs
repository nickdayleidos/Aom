using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Employer", Schema = "Employee")]
    public class Employer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;


    }
}
