using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Aws
{
    [Table("RoutingProfile", Schema = "Aws")]
    public class RoutingProfile
    {
        public int Id { get; set; }
        public string? Guid { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        // NEW: Navigation property
        public virtual ICollection<RoutingProfileQueue> RoutingProfileQueues { get; set; } = new List<RoutingProfileQueue>();
    }
}
