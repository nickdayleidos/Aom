using MyApplication.Components.Model.AOM.Aws;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("CertificationTypes", Schema = "Employee")]
    public class CertificationType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, Column(TypeName = "varchar(64)")]
        public string? Name { get; set; }
        [Required, Column(TypeName = "varchar(32)")]
        public string? ShortName  { get; set; }
        public int? VendorId { get; set; }
        public CertificationVendor? Vendor { get; set; }
        public int? IsContinuingEducation { get; set; }
        public int? IatLevel { get; set; }
        public int? IamLevel { get; set; }

    }

    [Table("CertificationVendors", Schema = "Employee")]
    public class CertificationVendor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, Column(TypeName = "varchar(64)")]
        public string? Name { get; set; }

    }

    [Table("Certifications", Schema = "Employee")]
    public class Certification
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }
        public int CertificationTypeId { get; set; }
        public CertificationType? CertificationType { get; set; }
        public DateOnly CertificationDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        [Required, Column(TypeName = "varchar(32)")]
        public string SerialNumber { get; set; }
        public DateOnly? CeRegistrationDate { get; set; }
        public DateTime UploadDate { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? UploadedBy { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string FileName { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string VerifiedBy { get; set; }
        public bool Verified { get; set; }
    }

}
