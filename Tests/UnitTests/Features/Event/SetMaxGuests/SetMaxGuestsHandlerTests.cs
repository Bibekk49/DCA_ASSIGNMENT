using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.SetMaxGuests;

public class SetMaxGuestsHandlerTests
{
    // S1: Draft event → max guests updated
    [Fact]
    public async Task GivenExistingDraftEvent_WhenHandlingSetMaxGuestsCommand_ThenMaxGuestsIsUpdated()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 25)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(25, repo.Events.First().MaxGuestNumber.Value);
    }

    // S2: Ready event → max guests updated, status reverts to DRAFT
    [Fact]
    public async Task GivenExistingReadyEvent_WhenHandlingSetMaxGuestsCommand_ThenMaxGuestsUpdatedAndStatusIsDraft()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 30)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(30, repo.Events.First().MaxGuestNumber.Value);
        Assert.Equal(EventStatus.DRAFT, repo.Events.First().Status);
    }

    // S3: Active event — increasing max guests is allowed
    [Fact]
    public async Task GivenActiveEventWithLowerValue_WhenIncreasingMaxGuests_ThenSuccess()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE; // default max is 5
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 30)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(30, repo.Events.First().MaxGuestNumber.Value);
        Assert.Equal(EventStatus.ACTIVE, repo.Events.First().Status);
    }

    // F1: Active event — reducing max guests is not allowed
    [Fact]
    public async Task GivenActiveEventWithHigherValue_WhenReducingMaxGuests_ThenFailureWithCannotReduce()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.MaxGuestNumber = ((Success<EventMaxGuests>)EventMaxGuests.Create(30)).Value;
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 10)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.MaxGuests.CannotReduceWhenActive);
    }

    // F2: Event not found → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingSetMaxGuestsCommand_ThenFailureWithNotFound()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(Guid.NewGuid().ToString(), 10)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }

    // F3: Completed event → failure with CannotModifyActive error
    [Fact]
    public async Task GivenCompletedEvent_WhenHandlingSetMaxGuestsCommand_ThenFailureWithCannotModifyActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.COMPLETED;
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 10)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F4: Cancelled event → failure with CannotModifyCancelled error
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingSetMaxGuestsCommand_ThenFailureWithCannotModifyCancelled()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<SetMaxGuestsCommand> handler = new SetMaxGuestsHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.CANCELLED;
        await repo.AddAsync(evt);

        var command = ((Success<SetMaxGuestsCommand>)SetMaxGuestsCommand.Create(evt.Id.Value.ToString(), 10)).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }
}
