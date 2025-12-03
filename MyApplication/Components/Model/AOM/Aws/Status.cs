using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Aws
{
    [Table("Status", Schema = "Aws")]
    public class Status
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
