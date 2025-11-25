using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("BreakLunchSchedules", Schema = "Employee")]
    public class BreakSchedules
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int BreakLength { get; set; }
        public int LunchLengh { get; set; }
        public TimeOnly LunchTime { get; set; }
        public TimeOnly Break1Time { get; set; }
        public TimeOnly Break2Time { get; set; }
        public TimeOnly Break3Time { get; set; }
        public bool? isMonday { get; set; }
        public bool? isTuesday { get; set; }
        public bool? isWednesday { get; set; }
        public bool? isThursday { get;set; }
        public bool? isFriday { get; set; }
        public bool? isSaturday { get;set; }
        public bool? isSunday { get;set; }

    }
}
