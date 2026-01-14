namespace MyApplication.Components.Service.Training.Certifications
{
    public class Current8570ReportItem
    {
        public int EmployeeId { get; set; }
        public int? CertificationId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = "N/A";

        public string RequiredLevel { get; set; } = "N/A";

        public string CertificationName { get; set; } = "N/A";
        public DateOnly? ExpirationDate { get; set; }
        public string SerialNumber { get; set; } = "N/A";

        public string AchievedLevel { get; set; } = "N/A";

        // NEW: Hire Date for display
        public DateOnly? HireDate { get; set; }
    }
}