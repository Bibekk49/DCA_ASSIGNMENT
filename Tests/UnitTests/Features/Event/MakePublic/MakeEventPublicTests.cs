using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.ViaEvent;

public class MakeEventPublicTests
{
    [Theory]
    [InlineData(EventStatus.DRAFT)]
    [InlineData(EventStatus.READY)]
    [InlineData(EventStatus.ACTIVE)]
    public void GivenDraftReadyOrActiveEvent_WhenMakingEventPublic_ThenEventIsPublicAndStatusIsUnchanged(EventStatus status)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.SetStatusForTesting(status);

        // Act
        Result<None> result = evt.MakePublic();

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PUBLIC, evt.EventVisibility);
        Assert.Equal(status, evt.Status);
    }

    [Fact]
    public void GivenCancelledEvent_WhenMakingEventPublic_ThenFailureExplainsCancelledEventCannotBeModified()
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.SetStatusForTesting(EventStatus.CANCELLED);

        // Act
        Result<None> result = evt.MakePublic();

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Equal(EventVisibility.PRIVATE, evt.EventVisibility);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }
}


