using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.UpdateDescription;

public class UpdateDescriptionHandlerTests
{
    // S1: Existing draft event, valid command → description updated, result is success
    [Fact]
    public async Task GivenExistingDraftEvent_WhenHandlingUpdateDescriptionCommand_ThenDescriptionIsUpdated()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateDescriptionCommand> handler = new UpdateDescriptionHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateDescriptionCommand>)UpdateDescriptionCommand.Create(evt.Id.Value.ToString(), "Updated description.")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal("Updated description.", repo.Events.First().Description.Value);
    }

    // S2: Existing ready event → description updated, status reverts to DRAFT
    [Fact]
    public async Task GivenExistingReadyEvent_WhenHandlingUpdateDescriptionCommand_ThenDescriptionUpdatedAndStatusIsDraft()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateDescriptionCommand> handler = new UpdateDescriptionHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateDescriptionCommand>)UpdateDescriptionCommand.Create(evt.Id.Value.ToString(), "Ready desc.")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal("Ready desc.", repo.Events.First().Description.Value);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // F1: Event not found → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingUpdateDescriptionCommand_ThenFailure()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateDescriptionCommand> handler = new UpdateDescriptionHandler(repo, uow);

        var command = ((Success<UpdateDescriptionCommand>)UpdateDescriptionCommand.Create(Guid.NewGuid().ToString(), "Some desc.")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }

    // F2: Active event → failure with CannotModifyActive error
    [Fact]
    public async Task GivenActiveEvent_WhenHandlingUpdateDescriptionCommand_ThenFailureWithCannotModifyActive()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateDescriptionCommand> handler = new UpdateDescriptionHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateDescriptionCommand>)UpdateDescriptionCommand.Create(evt.Id.Value.ToString(), "Active desc.")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F3: Cancelled event → failure with CannotModifyCancelled error
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingUpdateDescriptionCommand_ThenFailureWithCannotModifyCancelled()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateDescriptionCommand> handler = new UpdateDescriptionHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.CANCELLED;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateDescriptionCommand>)UpdateDescriptionCommand.Create(evt.Id.Value.ToString(), "Cancelled desc.")).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }
}
