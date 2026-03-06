using Dapper;
using MediatR;
using MyApplication.Components.Model.AOM.Tools;
using AomHomeEvent = MyApplication.Components.Model.AOM.Tools.HomeEvent;
using MyApplication.Components.Service;

namespace MyApplication.Components.Features.HomeEvents.Queries;

public record ListUpcomingEventsQuery : IRequest<IReadOnlyList<AomHomeEvent>>;

public sealed class ListUpcomingEventsQueryHandler
    : IRequestHandler<ListUpcomingEventsQuery, IReadOnlyList<AomHomeEvent>>
{
    private readonly IDbConnectionFactory _factory;
    public ListUpcomingEventsQueryHandler(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<AomHomeEvent>> Handle(ListUpcomingEventsQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT Id, Title, EventDate, Description, CreatedBy, CreatedAt
            FROM [Tools].[HomeEvent]
            WHERE CAST(EventDate AS date) >= CAST(GETDATE() AS date)
            ORDER BY EventDate;";

        await using var cn = _factory.Create("AOM");
        var rows = await cn.QueryAsync<AomHomeEvent>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
