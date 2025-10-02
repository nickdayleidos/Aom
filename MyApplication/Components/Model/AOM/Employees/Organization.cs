using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyApplication.Components.Model.AOM.Employee
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; } = string.Empty;
        [Column(TypeName = "varchar(128)")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}