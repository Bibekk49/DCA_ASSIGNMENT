using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventHandlerTests
{
    [Fact]
    public async Task GivenCreateCommand_WhenHandled_ThenEventIsAdded()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<CreateEventCommand> handler = new CreateEventHandler(repo, uow);

        var command = ((Success<CreateEventCommand>)CreateEventCommand.Create("Conference", "Annual event", 25)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Single(repo.Events);
        Assert.Equal(EventStatus.DRAFT, repo.Events[0].Status);
        Assert.Equal("Conference", repo.Events[0].Title.Value);
        Assert.Equal("Annual event", repo.Events[0].Description.Value);
        Assert.Equal(25, repo.Events[0].MaxGuestNumber.Value);
    }
}