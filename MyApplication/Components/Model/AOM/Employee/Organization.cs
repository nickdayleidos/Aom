using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyApplication.Components.Model.AOM.Aws;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Organization", Schema = "Employee")]
    public class Organization
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;

    }

    [Table("SubOrganization", Schema = "Employee")]
    public class SubOrganization
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string? Name { get; set; }
        public int? AwsStatusId { get; set; }
        public Status? AwsStatus { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? IatLevel { get; set; }
        public int? IamLevel { get; set; }
    }
}
