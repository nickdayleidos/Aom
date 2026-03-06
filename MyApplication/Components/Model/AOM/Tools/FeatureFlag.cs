using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools
{
    [Table("FeatureFlag", Schema = "Tools")]
    public class FeatureFlag
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Unique key for code lookups, e.g., "Module.ACR"</summary>
        [Required, MaxLength(100)]
        public string Key { get; set; } = null!;

        /// <summary>Human-readable label shown in admin UI</summary>
        [Required, MaxLength(200)]
        public string DisplayName { get; set; } = null!;

        /// <summary>Master on/off switch. False disables the feature for everyone.</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Optional comma-separated role names that bypass normal auth and gain access
        /// regardless of page-level [Authorize] restrictions.
        /// Null means no role override — use normal page authorization.
        /// Example: "Admin,WFM"
        /// </summary>
        [MaxLength(500)]
        public string? AllowedRoles { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}