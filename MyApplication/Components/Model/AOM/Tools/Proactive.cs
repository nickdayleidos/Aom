using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApplication.Components.Model.AOM.Tools
{
    public class ProactiveAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime ProactiveTime { get; set; }  // stamped in ET by repository on insert

        public string UsnInjectionAnnouncement { get; set; }
    
        public string UsnSiteAnnouncement { get; set; }
        public string UsnStatusAnnouncement { get; set; }
    }
}