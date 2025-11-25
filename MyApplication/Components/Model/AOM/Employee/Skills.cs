using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("Skills", Schema = "Employee")]
    public class Skills
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employee { get; set; }
        public int SkillTypeId { get; set; }
        public SkillType SkillType { get; set; }
        public DateOnly SkillDate {  get; set; }
        public bool? IsActive { get; set; }
    }
    [Table("SkillType", Schema = "Employee")]
    public class SkillType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
