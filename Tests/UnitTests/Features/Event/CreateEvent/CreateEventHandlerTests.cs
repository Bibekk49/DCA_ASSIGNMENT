using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventHandlerTests
{
    [Fact]
    public async Task GivenNothing_WhenCreatingEvent_ThenNewEventIsCreatedWithIdAndDefaultValues()
    {
        // arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<CreateEventCommand> handler = new CreateEventHandler(repo, uow);

        CreateEventCommand command = ((Success<CreateEventCommand>)CreateEventCommand.Create()).Value;

        // act
        var result = await handler.HandleAsync(command);

        // assert
        Assert.True(result is Success<None>);
        Assert.Single(repo.Events);

        var evt = repo.Events.First();
        Assert.Equal(command.Id, evt.Id);
    }
}