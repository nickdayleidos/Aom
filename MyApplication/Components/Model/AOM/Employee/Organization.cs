using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Organization", Schema = "Employee")]
    public class Organization
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; } = string.Empty;

        [Column(TypeName = "varchar(128)")]
        public string? ShortName { get; set; }

        public bool IsActive { get; set; } = true;

        // 🚫 Removed self-collection
    }
}
