using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("SubOrganization", Schema = "Employee")]
    public class SubOrganization
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; }

        [Column(TypeName = "varchar(128)")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        // 🚫 Removed self-collection
    }
}
