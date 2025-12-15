namespace MyApplication.Components.Model.AOM.Employee
{
    public class EmployeeCurrentDetails
    {
        public int EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleInitial { get; set; }
        public bool IsActive { get; set; }

        public DateOnly? EffectiveDate { get; set; }

        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }

        public int? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }

        public string? OrganizationName { get; set; }
        public string? SubOrganizationName { get; set; }
        public string? EmployerName { get; set; }
        public string? SiteName { get; set; }
    }
}