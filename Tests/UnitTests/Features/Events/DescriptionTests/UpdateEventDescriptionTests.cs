using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using EventAggregate = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Events.DescriptionTests;

public class UpdateEventDescriptionTests
{
    [Fact]
    public void UpdateDescription_WhenEventIsDraft_ShouldSucceed()
    {
        // Arrange
        Result<EventAggregate> eventResult = EventAggregate.Create();
        EventAggregate viaEvent = ((Success<EventAggregate>)eventResult).Value;

        Result<EventDescription> descriptionResult = EventDescription.Create("Updated description");
        EventDescription newDescription = ((Success<EventDescription>)descriptionResult).Value;

        // Act
        Result<None> result = viaEvent.UpdateDescription(newDescription);

        // Assert
        Assert.IsType<Success<None>>(result);
        Assert.Equal("Updated description", viaEvent.description.Value);
        Assert.Equal(EventStatus.DRAFT, viaEvent.status);
    }

    [Fact]
    public void UpdateDescription_WhenEventIsReady_ShouldSucceedAndSetStatusToDraft()
    {
        // Arrange
        Result<EventAggregate> eventResult = EventAggregate.Create();
        EventAggregate viaEvent = ((Success<EventAggregate>)eventResult).Value;

        SetStatus(viaEvent, EventStatus.READY);

        Result<EventDescription> descriptionResult = EventDescription.Create("Ready event updated");
        EventDescription newDescription = ((Success<EventDescription>)descriptionResult).Value;

        // Act
        Result<None> result = viaEvent.UpdateDescription(newDescription);

        // Assert
        Assert.IsType<Success<None>>(result);
        Assert.Equal("Ready event updated", viaEvent.description.Value);
        Assert.Equal(EventStatus.DRAFT, viaEvent.status);
    }

    [Fact]
    public void UpdateDescription_WhenEventIsActive_ShouldFail()
    {
        // Arrange
        Result<EventAggregate> eventResult = EventAggregate.Create();
        EventAggregate viaEvent = ((Success<EventAggregate>)eventResult).Value;

        SetStatus(viaEvent, EventStatus.ACTIVE);

        Result<EventDescription> descriptionResult = EventDescription.Create("Active event updated");
        EventDescription newDescription = ((Success<EventDescription>)descriptionResult).Value;

        // Act
        Result<None> result = viaEvent.UpdateDescription(newDescription);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, error => error == EventErrors.Status.CannotModifyActive);
    }

    [Fact]
    public void UpdateDescription_WhenEventIsCancelled_ShouldFail()
    {
        // Arrange
        Result<EventAggregate> eventResult = EventAggregate.Create();
        EventAggregate viaEvent = ((Success<EventAggregate>)eventResult).Value;

        SetStatus(viaEvent, EventStatus.CANCELLED);

        Result<EventDescription> descriptionResult = EventDescription.Create("Cancelled event updated");
        EventDescription newDescription = ((Success<EventDescription>)descriptionResult).Value;

        // Act
        Result<None> result = viaEvent.UpdateDescription(newDescription);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, error => error == EventErrors.Status.CannotModifyCancelled);
    }

    private static void SetStatus(EventAggregate viaEvent, EventStatus status)
    {
        var property = typeof(EventAggregate).GetProperty(nameof(EventAggregate.status));
        property!.SetValue(viaEvent, status);
    }
}