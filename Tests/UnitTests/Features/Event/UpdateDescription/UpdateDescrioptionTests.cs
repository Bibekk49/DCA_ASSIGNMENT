using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.UpdateDescription;

public class UpdateEventDescriptionTests
{
    // S1: Successfully update description on a Draft event
    [Theory]
    [InlineData("Updated description")]
    [InlineData("VIA hackathon details")]
    public void GivenDraftEventAndValidDescription_WhenUpdatingDescription_ThenDescriptionIsUpdated(string description)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        EventDescription newDescription = ((Success<EventDescription>)EventDescription.Create(description)).Value;

        // Act
        Result<None> result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(description, evt.Description.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    // S2: Successfully set description to empty
    [Fact]
    public void GivenEvent_WhenSettingEmptyDescription_ThenDescriptionIsUpdatedToEmpty()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        EventDescription newDescription = ((Success<EventDescription>)EventDescription.Create(string.Empty)).Value;

        // Act
        Result<None> result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(string.Empty, evt.Description.Value);
    }

    // S3: Successfully update description on a Ready event — event reverts to Draft
    [Theory]
    [InlineData("Ready event updated")]
    [InlineData("")]
    public void GivenReadyEventAndValidDescription_WhenUpdatingDescription_ThenDescriptionIsUpdatedAndStatusIsReverted(string description)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        EventDescription newDescription = ((Success<EventDescription>)EventDescription.Create(description)).Value;

        // Act
        Result<None> result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(description, evt.Description.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    // F1: Description is too long
    [Fact]
    public void GivenTooLongDescription_WhenCreatingDescription_ThenFailure()
    {
        string input = new string('a', 251);

        Result<EventDescription> result = EventDescription.Create(input);

        Assert.True(result is Failure<EventDescription>);
        Assert.Contains(((Failure<EventDescription>)result).Errors, e => e == EventErrors.Description.DescriptionTooLong);
    }

    // F2: Cannot update description on an Active event
    [Fact]
    public void GivenActiveEvent_WhenUpdatingDescription_ThenFailure()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        EventDescription newDescription = ((Success<EventDescription>)EventDescription.Create("Active event updated")).Value;

        // Act
        Result<None> result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F3: Cannot update description on a Cancelled event
    [Fact]
    public void GivenCancelledEvent_WhenUpdatingDescription_ThenFailure()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Cancel();
        EventDescription newDescription = ((Success<EventDescription>)EventDescription.Create("Cancelled event updated")).Value;

        // Act
        Result<None> result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }
}