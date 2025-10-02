using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools
{
    public class EmailTemplates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string TemplateName { get; set; } = string.Empty;
        [Column(TypeName = "varchar(128)")]
        public string Subject { get; set; } = string.Empty;
        public string ToAddresses { get; set; } = string.Empty;
        public string? CcAddresses { get; set; } = string.Empty;
        [Column(TypeName = "varchar(64)")]
        public string SendFromAddress { get; set; } = string.Empty;
        public string? Body { get; set; } = string.Empty;
        public bool IsActive { get; set; } 

    }
}
