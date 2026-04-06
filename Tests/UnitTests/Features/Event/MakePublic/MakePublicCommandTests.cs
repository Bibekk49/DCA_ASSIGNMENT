using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.MakePublic;

public class MakePublicCommandTests
{
    // S1: Valid eventId → command created successfully
    [Fact]
    public void GivenValidEventId_WhenCreatingCommand_ThenSuccess()
    {
        var result = MakePublicCommand.Create(Guid.NewGuid().ToString());

        Assert.True(result is Success<MakePublicCommand>);
        Assert.NotNull(((Success<MakePublicCommand>)result).Value.EventId);
    }

    // F1: Empty guid → failure with id error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = MakePublicCommand.Create(Guid.Empty.ToString());

        Assert.True(result is Failure<MakePublicCommand>);
        Assert.Contains(((Failure<MakePublicCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}
