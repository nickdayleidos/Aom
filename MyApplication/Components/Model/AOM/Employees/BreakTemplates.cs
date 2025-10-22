using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("BreakTemplates", Schema = "Employee")]
    public class BreakTemplates
    {
        
        public int Id { get; set; }
        public string Breaks { get; set; }
        public int BreakTime { get; set; }
        public int ShiftLength { get; set; }
        public TimeOnly Break1Time { get; set; }
        public TimeOnly? Break2Time { get; set; }
        public TimeOnly? Break3Time { get; set; }
        public TimeOnly? Break4Time { get; set; }
        public TimeOnly? Break5Time { get; set; }
        public TimeOnly? Break6Time { get; set; }
        public TimeOnly? Break7Time { get; set; }
        public TimeOnly? Break8Time { get; set; }
        public TimeOnly? LunchTime { get; set; }
        public string? WfmComment { get; set; }
    }
}
