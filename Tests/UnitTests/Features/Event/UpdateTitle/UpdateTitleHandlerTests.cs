using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.UpdateTitle;

public class UpdateTitleHandlerTests
{
    // S1: Existing draft event, valid command → title updated, result is success
    [Fact]
    public async Task GivenExistingDraftEvent_WhenHandlingUpdateTitleCommand_ThenTitleIsUpdated()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTitleCommand> handler = new UpdateTitleHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(evt.Id.Value.ToString(), "New Title")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal("New Title", repo.Events.First().Title.Value);
    }

    // S2: Existing ready event, valid command → title updated, status reverts to DRAFT
    [Fact]
    public async Task GivenExistingReadyEvent_WhenHandlingUpdateTitleCommand_ThenTitleIsUpdatedAndStatusIsDraft()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTitleCommand> handler = new UpdateTitleHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(evt.Id.Value.ToString(), "Ready Title")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal("Ready Title", repo.Events.First().Title.Value);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // F1: Event not found → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingUpdateTitleCommand_ThenFailure()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTitleCommand> handler = new UpdateTitleHandler(repo, uow);

        var command = ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(Guid.NewGuid().ToString(), "New Title")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }

    // F2: Active event → failure with CannotModifyActive error
    [Fact]
    public async Task GivenActiveEvent_WhenHandlingUpdateTitleCommand_ThenFailureWithCannotModifyActive()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTitleCommand> handler = new UpdateTitleHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(evt.Id.Value.ToString(), "Active Title")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F3: Cancelled event → failure with CannotModifyCancelled error
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingUpdateTitleCommand_ThenFailureWithCannotModifyCancelled()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTitleCommand> handler = new UpdateTitleHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.CANCELLED;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(evt.Id.Value.ToString(), "Cancelled Title")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }
}
