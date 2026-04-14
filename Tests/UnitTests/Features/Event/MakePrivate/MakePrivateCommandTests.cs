using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.MakePrivate;

public class MakePrivateCommandTests
{
    // S1: Valid eventId → command created successfully
    [Fact]
    public void GivenValidEventId_WhenCreatingCommand_ThenSuccess()
    {
        var result = MakePrivateCommand.Create(Guid.NewGuid().ToString());

        Assert.True(result is Success<MakePrivateCommand>);
        Assert.NotNull(((Success<MakePrivateCommand>)result).Value.EventId);
    }

    // F1: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = MakePrivateCommand.Create(Guid.Empty.ToString());

        Assert.True(result is Failure<MakePrivateCommand>);
        Assert.Contains(((Failure<MakePrivateCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}
