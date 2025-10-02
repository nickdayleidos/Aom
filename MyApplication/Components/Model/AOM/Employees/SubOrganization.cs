using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyApplication.Components.Model.AOM.Employee
{
    public class SubOrganization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public ICollection<SubOrganization> SubOrganizations { get; set; } = new List<SubOrganization>();
    }
}