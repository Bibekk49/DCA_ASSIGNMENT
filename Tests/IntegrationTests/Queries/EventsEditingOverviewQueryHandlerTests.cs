using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.Queries;

public class EventsEditingOverviewQueryHandlerTests : QueryTestBase
{
    [Fact]
    public async Task EventsEditingOverview_Returns_Three_Groups()
    {
        var handler = new EventsEditingOverviewQueryHandler(Context);
        var answer = await handler.HandleAsync(new EventsEditingOverview.Query());

        Assert.NotNull(answer);
        Assert.NotNull(answer.DraftEvents);
        Assert.NotNull(answer.ReadyEvents);
        Assert.NotNull(answer.CancelledEvents);
    }

    [Fact]
    public async Task EventsEditingOverview_Does_Not_Include_Active_Events()
    {
        var handler = new EventsEditingOverviewQueryHandler(Context);
        var answer = await handler.HandleAsync(new EventsEditingOverview.Query());

        var all = answer.DraftEvents.Concat(answer.ReadyEvents).Concat(answer.CancelledEvents).ToList();
        var activeEvents = await Context.Events
            .Where(e => e.Status == "active")
            .ToListAsync();

        foreach (var active in activeEvents)
            Assert.DoesNotContain(all, e => e.EventId == active.Id);
    }
}
