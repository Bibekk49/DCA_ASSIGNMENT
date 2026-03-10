using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.Features.Events.CreateEvent;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void GivenCreator_WhenCreateEvent_ThenDraftStatusAndDefaultsAreSet()
    {
        Result<ViaEvent> result = ViaEvent.Create();

        var success = Assert.IsType<Success<ViaEvent>>(result);
        var evt = success.Value;

        Assert.NotEqual(Guid.Empty, evt.Id.Value);
        Assert.Equal(EventStatus.Draft, evt.Status);
        Assert.Equal(5, evt.MaxGuests.Value);
    }
}