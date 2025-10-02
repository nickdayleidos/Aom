using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDetailsDto>> GetAllEmployeesAsync();
        Task<EmployeeDetailsDto> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(CreateEmployeeDto  createEmployee);
        Task DeleteEmployeeAsync(int id);
        Task EditEmployeeAsync(UpdateEmployeeDto  EditEmployee);
    }
}