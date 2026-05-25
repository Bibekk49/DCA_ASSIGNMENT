using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.Queries;

public class BrowseUpcomingEventsQueryHandlerTests : QueryTestBase
{
    private readonly FakeSystemTime _time = new(new DateTime(2024, 1, 1));

    [Fact]
    public async Task BrowseUpcomingEvents_Returns_Active_Future_Events()
    {
        var handler = new BrowseUpcomingEventsQueryHandler(Context, _time);
        var answer = await handler.HandleAsync(new BrowseUpcomingEvents.Query(1, 10, null));

        Assert.NotNull(answer);
        Assert.True(answer.Events.Count > 0);
        Assert.All(answer.Events, e => Assert.NotEmpty(e.Visibility));
    }

    [Fact]
    public async Task BrowseUpcomingEvents_Paging_Works()
    {
        var handler = new BrowseUpcomingEventsQueryHandler(Context, _time);
        var page1 = await handler.HandleAsync(new BrowseUpcomingEvents.Query(1, 3, null));
        var page2 = await handler.HandleAsync(new BrowseUpcomingEvents.Query(2, 3, null));

        Assert.Equal(3, page1.Events.Count);
        Assert.True(page1.Events[0].EventId != page2.Events[0].EventId);
    }

    [Fact]
    public async Task BrowseUpcomingEvents_TitleSearch_Filters()
    {
        var handler = new BrowseUpcomingEventsQueryHandler(Context, _time);
        var answer = await handler.HandleAsync(new BrowseUpcomingEvents.Query(1, 10, "Friday"));

        Assert.All(answer.Events, e => Assert.Contains("Friday", e.Title));
    }

    [Fact]
    public async Task BrowseUpcomingEvents_Returns_Correct_TotalPages()
    {
        var handler = new BrowseUpcomingEventsQueryHandler(Context, _time);
        var answer = await handler.HandleAsync(new BrowseUpcomingEvents.Query(1, 3, null));

        Assert.True(answer.TotalPages >= 1);
    }
}
