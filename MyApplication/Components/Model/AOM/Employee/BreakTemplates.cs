using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("BreakTemplates", Schema = "Employee")]
    public class BreakTemplates
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ShiftLength { get; set; }
        public int? Breaks { get; set; }
        public int? BreakLength { get; set; }
        public int? LunchLength { get; set; }
        public int? LunchTime { get; set; }
        public int? Break1Time { get; set; }
        public int? Break2Time { get; set; }
        public int? Break3Time { get; set; }
        public int? Break4Time { get; set; }
        public int? Break5Time { get; set; }
        public int? Break6Time { get; set; }
        public int? Break7Time { get; set; }
        public int? Break8Time { get; set; }

        public bool? IsActive { get; set; } = true;

    }
}
