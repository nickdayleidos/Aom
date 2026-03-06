using Dapper;
using MediatR;
using MyApplication.Components.Service;

namespace MyApplication.Components.Features.HomeEvents.Commands;

public record DeleteHomeEventCommand(int Id) : IRequest;

public sealed class DeleteHomeEventCommandHandler : IRequestHandler<DeleteHomeEventCommand>
{
    private readonly IDbConnectionFactory _factory;
    public DeleteHomeEventCommandHandler(IDbConnectionFactory factory) => _factory = factory;

    public async Task Handle(DeleteHomeEventCommand request, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM [Tools].[HomeEvent] WHERE Id = @id;";
        await using var cn = _factory.Create("AOM");
        await cn.ExecuteAsync(new CommandDefinition(sql, new { id = request.Id }, cancellationToken: cancellationToken));
    }
}
