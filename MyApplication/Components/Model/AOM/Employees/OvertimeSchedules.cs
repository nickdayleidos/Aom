using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("OvertimeSchedules", Schema = "Employee")]
    public class OvertimeSchedules
    {
        public int Id { get; set; }
        public TimeOnly Duration { get; set; }
        public int EmployeeId { get; set; }
        public TimeOnly BeforeShiftMonday { get; set; }
        public TimeOnly AfterShiftMonday { get; set; }
        public TimeOnly BeforeShiftTuesday { get; set; }
        public TimeOnly AfterShiftTuesday { get; set; }
        public TimeOnly BeforeShiftWednesday { get; set; }
        public TimeOnly AfterShiftWednesday { get; set; }
        public TimeOnly BeforeShiftThursday { get; set; }
        public TimeOnly AfterShiftThursday { get; set; }
        public TimeOnly BeforeShiftFriday { get; set; }
        public TimeOnly AfterShiftFriday { get; set; }
        public TimeOnly BeforeShiftSaturday { get; set; }
        public TimeOnly AfterShiftSaturday { get; set; }
        public TimeOnly BeforeShiftSunday { get; set; }
        public TimeOnly AfterShiftSunday { get; set; }

    }
}