using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Model.AOM.Aws
{
    public class Identifiers
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string AwsUsername { get; set; }
        public int? EmployeeId { get; set; }
        public Employees? Employee { get; set; }
    }
}
