using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Aws
{
    [Table("RoutingProfileQueue", Schema = "Aws")]
    public class RoutingProfileQueue
    {
        public int Id { get; set; }
        public int RoutingProfileId { get; set; }
        public RoutingProfile? RoutingProfile { get; set; }
        public int CallQueueId { get; set; }
        public CallQueue? CallQueue { get; set; }
        public int? Priority { get; set; }
        public int? Delay { get; set; }
        public bool? IsActive { get; set; }
    }
}
