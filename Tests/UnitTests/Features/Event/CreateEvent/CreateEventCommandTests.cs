using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventCommandTests
{
    [Fact]
    public void GivenNothing_WhenCreatingCommand_ThenSuccess()
    {
        Result<CreateEventCommand> result = CreateEventCommand.Create();
        CreateEventCommand command = ((Success<CreateEventCommand>)result).Value;

        Assert.True(result is Success<CreateEventCommand>);
        Assert.NotNull(command.Id);
        Assert.NotNull(command.Id.ToString());
        Assert.NotEmpty(command.Id.ToString()!);
    }
}