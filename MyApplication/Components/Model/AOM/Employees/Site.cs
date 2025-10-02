using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApplication.Components.Model.AOM.Employee
{
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? Location { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string? SiteCode { get; set; }
        public ICollection<Site> Sites { get; set; } = new List<Site>();
    }
}