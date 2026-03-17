using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.Features.Events.CreateEvent;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenStatusIsDraft()
    {
        Result<ViaEvent> event = ViaEvent.Create().Value;

        var evt = success.Value;

        Assert.NotEqual(Guid.Empty, evt.Id.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
        Assert.Equal(5, evt.MaxGuestNumber.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenTitleIsWorkingTitle()
    {
        Result<ViaEvent> event = ViaEvent.Create(eventId).Value;

        var evt = success.Value;

        Assert.Equal("Working Title", evt.Title.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenDescriptionIsEmpty()
    {
        Result<ViaEvent> @event = ViaEvent.Create(eventId);

        var success = Assert.IsType<Success<ViaEvent>>(@event);
        var evt = success.Value;

        Assert.Null(evt.Description);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenVisibilityIsPrivate()
    {

        Result<ViaEvent> @event = ViaEvent.Create(eventId);
        var evt = success.Value;

        Assert.Equal(EventVisibility.PRIVATE, evt.Visibility);
    }
}