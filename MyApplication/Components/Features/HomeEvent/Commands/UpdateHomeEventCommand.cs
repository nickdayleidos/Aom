using Dapper;
using MediatR;
using MyApplication.Components.Model.AOM.Tools;
using AomHomeEvent = MyApplication.Components.Model.AOM.Tools.HomeEvent;
using MyApplication.Components.Service;

namespace MyApplication.Components.Features.HomeEvents.Commands;

public record UpdateHomeEventCommand(AomHomeEvent Event) : IRequest;

public sealed class UpdateHomeEventCommandHandler : IRequestHandler<UpdateHomeEventCommand>
{
    private readonly IDbConnectionFactory _factory;
    public UpdateHomeEventCommandHandler(IDbConnectionFactory factory) => _factory = factory;

    public async Task Handle(UpdateHomeEventCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            UPDATE [Tools].[HomeEvent]
            SET Title = @Title, EventDate = @EventDate, Description = @Description
            WHERE Id = @Id;";

        await using var cn = _factory.Create("AOM");
        await cn.ExecuteAsync(new CommandDefinition(sql, request.Event, cancellationToken: cancellationToken));
    }
}
