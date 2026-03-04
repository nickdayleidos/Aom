using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Security
{
    [Table("AppRole", Schema = "Security")]
    public class AppRole
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } // e.g., "Admin", "Manager", "WFM"
    }

    [Table("AppRoleAssignment", Schema = "Security")]
    public class AppRoleAssignment
    {
        [Key]
        public int Id { get; set; }

        public int AppRoleId { get; set; }
        public AppRole? AppRole { get; set; }

        // This can be a specific username (LEIDOS-CORP\dayng) 
        // OR an AD Group Name (LEIDOS-CORP\SMIT_TODAdmin)
        [Required, MaxLength(128)]
        public string Identifier { get; set; }

        // Helper to know if this is a user or group mapping (optional, but good for UI)
        [MaxLength(20)]
        public string Type { get; set; } = "User"; // "User" or "Group"
    }
}