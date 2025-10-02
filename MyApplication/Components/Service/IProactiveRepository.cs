using System.Threading;
using System.Threading.Tasks;
using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service
{
    public interface IProactiveRepository
    {
        Task<ProactiveAnnouncement?> GetLatestAsync(CancellationToken ct = default);
        Task<ProactiveAnnouncement> InsertAsync(ProactiveAnnouncement entity, CancellationToken ct = default);
    }
}
