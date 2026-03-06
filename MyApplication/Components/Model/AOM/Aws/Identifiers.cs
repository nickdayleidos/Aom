using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Model.AOM.Aws
{
    public class Identifiers
    {
        public int Id { get; set; }
        public string Guid { get; set; } = null!;
        public string AwsUsername { get; set; } = null!;
        public int? EmployeeId { get; set; }
        public Employees? Employee { get; set; }
    }
}
