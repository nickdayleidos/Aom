using Dapper;
using MediatR;
using MyApplication.Components.Model.AOM.Tools;
using AomHomeEvent = MyApplication.Components.Model.AOM.Tools.HomeEvent;
using MyApplication.Components.Service;

namespace MyApplication.Components.Features.HomeEvents.Commands;

public record CreateHomeEventCommand(AomHomeEvent Event) : IRequest<int>;

public sealed class CreateHomeEventCommandHandler : IRequestHandler<CreateHomeEventCommand, int>
{
    private readonly IDbConnectionFactory _factory;
    public CreateHomeEventCommandHandler(IDbConnectionFactory factory) => _factory = factory;

    public async Task<int> Handle(CreateHomeEventCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO [Tools].[HomeEvent] (Title, EventDate, Description, CreatedBy, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@Title, @EventDate, @Description, @CreatedBy, @CreatedAt);";

        await using var cn = _factory.Create("AOM");
        return await cn.ExecuteScalarAsync<int>(new CommandDefinition(sql, request.Event, cancellationToken: cancellationToken));
    }
}
