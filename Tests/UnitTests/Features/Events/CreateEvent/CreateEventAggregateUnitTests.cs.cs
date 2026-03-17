using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Events.CreateEvent;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenStatusIsDraft()
    {
        Result<DomainEvent> result = DomainEvent.Create();

        var success = Assert.IsType<Success<DomainEvent>>(result);
        var evt = success.Value;

        Assert.NotEqual(Guid.Empty, evt.Id.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
        Assert.Equal(5, evt.MaxGuestNumber.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenTitleIsWorkingTitle()
    {
        var result = DomainEvent.Create();

        var success = Assert.IsType<Success<DomainEvent>>(result);
        var evt = success.Value;

        Assert.Equal("Working Title", evt.Title.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenDescriptionIsEmpty()
    {
        var result = DomainEvent.Create();

        var success = Assert.IsType<Success<DomainEvent>>(result);
        var evt = success.Value;

        Assert.Equal(string.Empty, evt.Description.Value);
    }

    [Fact]
    public void GivenEventId_WhenCreateEvent_ThenVisibilityIsPrivate()
    {
        var result = DomainEvent.Create();

        var success = Assert.IsType<Success<DomainEvent>>(result);
        var evt = success.Value;

        Assert.Equal(EventVisibility.PRIVATE, evt.EventVisibility);
    }
}