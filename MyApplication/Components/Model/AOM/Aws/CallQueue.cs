using MyApplication.Components.Model.AOM.Employee;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Aws
{
    [Table("CallQueue", Schema = "Aws")]
    public class CallQueue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SkillTypeId { get; set; }
        public SkillType? SkillType { get; set; }
        public bool? IsActive { get; set; }
    }
}
