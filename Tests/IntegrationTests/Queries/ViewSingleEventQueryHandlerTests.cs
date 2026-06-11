using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.Queries;

public class ViewSingleEventQueryHandlerTests : QueryTestBase
{
    private const string FridayBarEventId = "40ed2fd9-2240-4791-895f-b9da1a1f64e4";

    [Fact]
    public async Task ViewSingleEvent_Returns_Event_Details()
    {
        var handler = new ViewSingleEventQueryHandler(Context);
        var answer = await handler.HandleAsync(new ViewSingleEvent.Query(FridayBarEventId));

        Assert.NotNull(answer);
        Assert.Equal(FridayBarEventId, answer.EventId);
        Assert.Equal("Friday Bar", answer.Title);
    }

    [Fact]
    public async Task ViewSingleEvent_Throws_For_Unknown_Event()
    {
        var handler = new ViewSingleEventQueryHandler(Context);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ViewSingleEvent.Query("00000000-0000-0000-0000-000000000000")));
    }
}
