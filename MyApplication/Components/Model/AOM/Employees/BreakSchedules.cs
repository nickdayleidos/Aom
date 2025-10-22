using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Employee
{
    [Table("BreakLunchSchedules", Schema = "Employee")]
    public class BreakSchedules
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int MondayTemplateId { get; set; }
        public int TuesdayTemplateId { get; set; }
        public int WednesdayTemplateId { get;set; }
        public int ThursdayTemplateId { get; set; }
        public int FridayTemplateId { get; set;}
        public int SaturdayTemplateId { get; set; }
        public int SundayTemplateId { get; set; }
    }
}
