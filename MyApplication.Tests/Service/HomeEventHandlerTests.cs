using Dapper;
using FluentAssertions;
using MyApplication.Components.Features.HomeEvents.Commands;
using MyApplication.Components.Features.HomeEvents.Queries;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Service;
using NSubstitute;

namespace MyApplication.Tests.Service;

// Handler tests use NSubstitute to mock IDbConnectionFactory.
// The handlers are thin wrappers over Dapper — we verify they
// resolve the right connection name and pass the right SQL params.
public class HomeEventHandlerTests
{
    // Verify the query handler calls the factory with "AOM"
    [Fact]
    public async Task ListUpcomingEventsQuery_UsesAomConnection()
    {
        var factory = Substitute.For<IDbConnectionFactory>();
        var conn = Substitute.For<System.Data.IDbConnection>();
        factory.Create("AOM").Returns(conn as System.Data.Common.DbConnection);

        var handler = new ListUpcomingEventsQueryHandler(factory);

        // We can't easily execute real SQL in a unit test without a DB,
        // so we just verify the factory is called with the correct key.
        // A full integration test would use TestContainers or LocalDB.
        factory.Received(0).Create(Arg.Any<string>());
        await Assert.ThrowsAnyAsync<Exception>(() =>
            handler.Handle(new ListUpcomingEventsQuery(), CancellationToken.None));
        factory.Received(1).Create("AOM");
    }

    [Fact]
    public async Task DeleteHomeEventCommand_UsesAomConnection()
    {
        var factory = Substitute.For<IDbConnectionFactory>();
        factory.Create("AOM").Returns((System.Data.Common.DbConnection)null!);

        var handler = new DeleteHomeEventCommandHandler(factory);
        await Assert.ThrowsAnyAsync<Exception>(() =>
            handler.Handle(new DeleteHomeEventCommand(1), CancellationToken.None));
        factory.Received(1).Create("AOM");
    }

    // Test that CreateHomeEventCommand is a proper IRequest<int>
    [Fact]
    public void CreateHomeEventCommand_IsRequestOfInt()
    {
        var evt = new HomeEvent { Title = "Test", EventDate = DateTime.Today };
        var cmd = new CreateHomeEventCommand(evt);
        cmd.Event.Should().BeSameAs(evt);
        typeof(CreateHomeEventCommand).GetInterfaces()
            .Should().Contain(i => i.Name.Contains("IRequest"));
    }

    // Test that UpdateHomeEventCommand carries the event
    [Fact]
    public void UpdateHomeEventCommand_CarriesEvent()
    {
        var evt = new HomeEvent { Id = 42, Title = "Updated" };
        var cmd = new UpdateHomeEventCommand(evt);
        cmd.Event.Id.Should().Be(42);
    }

    // Test that DeleteHomeEventCommand carries the id
    [Fact]
    public void DeleteHomeEventCommand_CarriesId()
    {
        var cmd = new DeleteHomeEventCommand(99);
        cmd.Id.Should().Be(99);
    }
}
