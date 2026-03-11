using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.ViaEvent;

public class UpdateEventTitleTests
{
    // S1: Successfully update title on a Draft event
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void GivenDraftEventAndValidTitle_WhenUpdatingTitle_ThenTitleIsUpdated(string titleStr)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create(titleStr)).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(titleStr, evt.Title.Value);
    }

    // S2: Successfully update title on a Ready event — event reverts to Draft
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void GivenReadyEventAndValidTitle_WhenUpdatingTitle_ThenTitleIsUpdatedAndStatusIsReverted(string titleStr)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.SetStatusForTesting(EventStatus.READY);
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create(titleStr)).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(titleStr, evt.Title.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    // F5: Cannot update title on an Active event
    [Fact]
    public void GivenActiveEvent_WhenUpdatingTitle_ThenFailure()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.SetStatusForTesting(EventStatus.ACTIVE);
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create("Some New Title")).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F6: Cannot update title on a Cancelled event
    [Fact]
    public void GivenCancelledEvent_WhenUpdatingTitle_ThenFailure()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Cancel();
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create("Some New Title")).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F5 (Completed): Cannot update title on a Completed event
    [Fact]
    public void GivenCompletedEvent_WhenUpdatingTitle_ThenFailure()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.SetStatusForTesting(EventStatus.COMPLETED);
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create("Some New Title")).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }
}


