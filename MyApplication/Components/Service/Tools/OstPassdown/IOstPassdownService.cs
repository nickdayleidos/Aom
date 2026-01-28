using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service.Tools.OstPassdown
{
    public interface IOstPassdownService
    {
        Task<List<Model.AOM.Tools.OstPassdown>> GetHistoryAsync();
        Task<Model.AOM.Tools.OstPassdown?> GetCurrentPassdownAsync();
        Task<Model.AOM.Tools.OstPassdown?> GetPassdownByIdAsync(int id);
        Task CreatePassdownAsync(Model.AOM.Tools.OstPassdown passdown, string user);
        Task UpdatePassdownAsync(Model.AOM.Tools.OstPassdown passdown, string user);
        Task<List<Employees>> GetOstEmployeesAsync();
    }
}