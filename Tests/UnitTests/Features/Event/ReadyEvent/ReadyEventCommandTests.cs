using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.ReadyEvent;

public class ReadyEventCommandTests
{
    // S1: Valid event id → command created successfully
    [Fact]
    public void GivenValidEventId_WhenCreatingCommand_ThenSuccess()
    {
        var result = ReadyEventCommand.Create(Guid.NewGuid().ToString());

        Assert.True(result is Success<ReadyEventCommand>);
        Assert.NotNull(((Success<ReadyEventCommand>)result).Value.EventId);
    }

    // F1: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = ReadyEventCommand.Create(Guid.Empty.ToString());

        Assert.True(result is Failure<ReadyEventCommand>);
        Assert.Contains(((Failure<ReadyEventCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}