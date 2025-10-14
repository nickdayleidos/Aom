using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Site", Schema = "Employee")]
    public class Site
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string? Location { get; set; }

        [Column(TypeName = "varchar(4)")]
        public string? SiteCode { get; set; }

        // 🚫 Removed self-collection
    }
}
