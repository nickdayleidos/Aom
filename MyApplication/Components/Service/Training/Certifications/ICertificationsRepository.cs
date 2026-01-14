using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Training.Certifications
{
    public interface ICertificationsRepository
    {
        Task<List<Certification>> GetAllCertificationsAsync();
        Task<Certification?> GetCertificationByIdAsync(int id);
        Task<List<CertificationType>> GetCertificationTypesAsync();
        Task<List<Employees>> GetEmployeesAsync();

        Task<List<EmployeeCurrentDetails>> GetEmployeeCurrentDetailsAsync();

        // New Report Method
        Task<List<Current8570ReportItem>> GetCurrent8570ReportAsync();

        // For filters
        Task<List<Manager>> GetManagersAsync();
        Task<List<Supervisor>> GetSupervisorsAsync();
        Task<List<Organization>> GetOrganizationsAsync();
        Task<List<SubOrganization>> GetSubOrganizationsAsync();

        Task AddCertificationAsync(Certification certification);
        Task UpdateCertificationAsync(Certification certification);
        Task DeleteCertificationAsync(int id);
    }
}