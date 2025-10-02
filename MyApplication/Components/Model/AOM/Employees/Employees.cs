using MyApplication.Components.Pages.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;


namespace MyApplication.Components.Model.AOM.Employee
{
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Changed from Guid to int
        [Required, Column(TypeName = "varchar(64)")]
        public string? FirstName { get; set; }
        [Required, Column(TypeName = "varchar(64)")]
        public string? LastName { get; set; }
        [Column(TypeName = "varchar(1)")]
        public string? MiddleInitial { get; set; }
        public int? SiteId { get; set; } = null;
        [Column(TypeName = "varchar(64)")]
        public string? NmciEmail { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? UsnOperatorId { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? UsnAdminId { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? FlankspeedEmail { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? CorporateEmail { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? CorporateId { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? DomainLoginName { get; set; }
        public ICollection<Employees> EmployeeLookup { get; set; } = new List<Employees>();

    }
}