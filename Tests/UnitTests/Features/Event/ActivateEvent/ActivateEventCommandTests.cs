using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.ActivateEvent;

public class ActivateEventCommandTests
{
    // S1: Valid event id → command created successfully
    [Fact]
    public void GivenValidEventId_WhenCreatingCommand_ThenSuccess()
    {
        var result = ActivateEventCommand.Create(Guid.NewGuid().ToString());

        Assert.True(result is Success<ActivateEventCommand>);
        Assert.NotNull(((Success<ActivateEventCommand>)result).Value.EventId);
    }

    // F1: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = ActivateEventCommand.Create(Guid.Empty.ToString());

        Assert.True(result is Failure<ActivateEventCommand>);
        Assert.Contains(((Failure<ActivateEventCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}