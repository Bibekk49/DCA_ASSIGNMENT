using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.Features.Events.CreateEvent;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenStatusIsDraft()
    {
        Result<DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent> result = ViaEvent.Create();

        var success = Assert.IsType<Success<DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent>>(result);
        var evt = success.Value;

        Assert.NotEqual(Guid.Empty, evt.Id.Value);
        Assert.Equal(EventStatus.DRAFT, evt.status);
        Assert.Equal(5, evt.maxGuestNumber.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenTitleIsWorkingTitle()
    {
        var result = ViaEvent.Create();

        var success = Assert.IsType<Success<ViaEvent>>(result);
        var evt = success.Value;

        Assert.Equal("Working Title", evt.title.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenDescriptionIsEmpty()
    {
        var result = ViaEvent.Create();

        var success = Assert.IsType<Success<ViaEvent>>(result);
        var evt = success.Value;

        Assert.Null(evt.description);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenVisibilityIsPrivate()
    {
        var result = ViaEvent.Create();

        var success = Assert.IsType<Success<ViaEvent>>(result);
        var evt = success.Value;

        Assert.Equal(EventVisibility.PRIVATE, evt.visibility);
    }
}