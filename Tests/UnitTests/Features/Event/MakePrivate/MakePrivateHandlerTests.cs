using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.MakePrivate;

public class MakePrivateHandlerTests
{
    // S1: Draft/ready event already private → nothing changes, status unchanged
    [Fact]
    public async Task GivenReadyEventAlreadyPrivate_WhenHandlingMakePrivateCommand_ThenNothingChanges()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        // EventVisibility is already PRIVATE by default
        await repo.AddAsync(evt);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PRIVATE, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.READY, repo.Events.First().Status); // status unchanged
    }

    // S2: Draft event, public → made private, status stays DRAFT
    [Fact]
    public async Task GivenDraftPublicEvent_WhenHandlingMakePrivateCommand_ThenEventIsPrivateAndStatusIsDraft()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.EventVisibility = EventVisibility.PUBLIC;
        await repo.AddAsync(evt);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PRIVATE, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // S2: Ready event, public → made private, status reverts to DRAFT
    [Fact]
    public async Task GivenReadyPublicEvent_WhenHandlingMakePrivateCommand_ThenEventIsPrivateAndStatusIsDraft()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        evt.EventVisibility = EventVisibility.PUBLIC;
        await repo.AddAsync(evt);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(EventVisibility.PRIVATE, repo.Events.First().EventVisibility);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // F1: Active event → failure with CannotModifyActive error
    [Fact]
    public async Task GivenActiveEvent_WhenHandlingMakePrivateCommand_ThenFailureWithCannotModifyActive()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F2: Cancelled event → failure with CannotModifyCancelled error
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingMakePrivateCommand_ThenFailureWithCannotModifyCancelled()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.CANCELLED;
        await repo.AddAsync(evt);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(evt.Id.Value.ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F3: Event not found → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingMakePrivateCommand_ThenFailure()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<MakePrivateCommand> handler = new MakePrivateHandler(repo, uow);

        var command = ((Success<MakePrivateCommand>)MakePrivateCommand.Create(Guid.NewGuid().ToString())).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }
}
