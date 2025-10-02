
using MyApplication.Components.Data;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service
{
    
    public class EmployeeService : IEmployeeService
    {

        private readonly AomDbContext _appDbContext;

        public EmployeeService(AomDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
     
public async Task CreateEmployeeAsync(CreateEmployeeDto createEmployee)
{
    var employee = new Employees
    {
        LastName = createEmployee.LastName,
        FirstName = createEmployee.FirstName,
        MiddleInitial = createEmployee.MiddleInitial,
        SiteId = createEmployee.SiteId,
       
    };

            _appDbContext.Employees.Add(employee);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDetailsDto>> GetAllEmployeesAsync()
        {
            return await _appDbContext.Employees
                .Select(e => new EmployeeDetailsDto
                {
                    Id = e.Id,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    MiddleInitial = e.MiddleInitial,
                    SiteId = e.SiteId,
                  
                })
                .ToListAsync();
        }

        public async Task<EmployeeDetailsDto> GetEmployeeByIdAsync(int id) // int here
        {
            var employee = await _appDbContext.Employees.FindAsync(id) ?? throw new KeyNotFoundException("Employee not found");
            return new EmployeeDetailsDto
            {
                Id = employee.Id,
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                MiddleInitial = employee.MiddleInitial,
                SiteId = employee.SiteId,
               
            };
        }

        public async Task EditEmployeeAsync(UpdateEmployeeDto editEmployee) // int in DTO
        {
            var employee = await _appDbContext.Employees.FindAsync(editEmployee.Id) ?? throw new KeyNotFoundException("Employee not found");
            employee.LastName = editEmployee.LastName;
            employee.FirstName = editEmployee.FirstName;
            employee.MiddleInitial = editEmployee.MiddleInitial;
            employee.SiteId = editEmployee.SiteId;
            

            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id) // int here
        {
            var employee = await _appDbContext.Employees.FindAsync(id) ?? throw new KeyNotFoundException("Employee not found");
            _appDbContext.Employees.Remove(employee);
            await _appDbContext.SaveChangesAsync();
        }
    }
}