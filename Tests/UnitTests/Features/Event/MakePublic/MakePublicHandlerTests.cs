using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.MakePublic;

public class MakePublicHandlerTests
{
    // S1a: Existing draft event → visibility set to public, result is success
    [Fact]
    public async Task GivenExistingDraftEvent_WhenHandlingMakePublicCommand_ThenEventIsPublic()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePublicCommand> handler = new MakePublicHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        await repo.AddAsync(evt);

        var command = ((Success<MakePublicCommand>)MakePublicCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PUBLIC, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // S1b: Existing ready event → visibility set to public, status remains READY
    [Fact]
    public async Task GivenExistingReadyEvent_WhenHandlingMakePublicCommand_ThenEventIsPublicAndStatusUnchanged()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePublicCommand> handler = new MakePublicHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        await repo.AddAsync(evt);

        var command = ((Success<MakePublicCommand>)MakePublicCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PUBLIC, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.READY, repo.Events.First().Status);
    }

    // S1c: Existing active event → visibility set to public, status remains ACTIVE
    [Fact]
    public async Task GivenExistingActiveEvent_WhenHandlingMakePublicCommand_ThenEventIsPublicAndStatusUnchanged()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePublicCommand> handler = new MakePublicHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<MakePublicCommand>)MakePublicCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PUBLIC, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.ACTIVE, repo.Events.First().Status);
    }

    // F1: Cancelled event → failure with CannotModifyCancelled error
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingMakePublicCommand_ThenFailureWithCannotModifyCancelled()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePublicCommand> handler = new MakePublicHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.CANCELLED;
        await repo.AddAsync(evt);

        var command = ((Success<MakePublicCommand>)MakePublicCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F2: Event not found → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingMakePublicCommand_ThenFailure()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePublicCommand> handler = new MakePublicHandler(repo, uow);

        var command = ((Success<MakePublicCommand>)MakePublicCommand.Create(Guid.NewGuid().ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }
}
