using MyApplication.Components.Data;
using Microsoft.EntityFrameworkCore;

namespace MyApplication.Components.Service
{
    // Split into partial files:
    //   OperaRepository.Queries.cs  — all read-only methods
    //   OperaRepository.Commands.cs — all write methods + business rules
    public sealed partial class OperaRepository : IOperaRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public OperaRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;
    }
}
