using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Service.Employee;
using MyApplication.Migrations;

namespace MyApplication.Components.Service.Acr;

// Split into partial files:
//   AcrQueryService.Search.cs   – SearchAsync, QueryAsync
//   AcrQueryService.Lookups.cs  – lookup methods
//   AcrQueryService.Details.cs  – LoadDetailsAsync, LoadForEditAsync, GetPrevDetailsAsync, helpers
public sealed partial class AcrQueryService : IAcrQueryService
{
    private readonly IDbContextFactory<AomDbContext> _dbFactory;
    private readonly ITimeDisplayService _timeService;

    public AcrQueryService(
        IDbContextFactory<AomDbContext> dbFactory,
        ITimeDisplayService timeService)
    {
        _dbFactory = dbFactory;
        _timeService = timeService;
    }
}
