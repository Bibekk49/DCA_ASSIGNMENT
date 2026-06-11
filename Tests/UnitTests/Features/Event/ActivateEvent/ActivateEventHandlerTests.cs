using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.ActivateEvent;

public class ActivateEventHandlerTests
{
    private static readonly DateTime FixedNow = new(2027, 1, 1, 10, 0, 0);

    private static EventTimes ValidFutureTimes() =>
        ((Success<EventTimes>)EventTimes.Create(
            new DateOnly(2027, 8, 25),
            new TimeOnly(10, 0),
            new DateOnly(2027, 8, 25),
            new TimeOnly(14, 0))).Value;

    private static DomainEvent ReadyableEvent()
    {
        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Summer Gala")).Value;
        evt.UpdateTitle(title);
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);
        return evt;
    }

    // S1: Valid draft event → auto-readied then ACTIVE
    [Fact]
    public async Task GivenValidDraftEvent_WhenHandlingActivateEventCommand_ThenStatusIsActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, repo.Events.First().Status);
    }

    // S2: Ready event → status becomes ACTIVE
    [Fact]
    public async Task GivenReadyEvent_WhenHandlingActivateEventCommand_ThenStatusIsActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        evt.MakeReady(FixedNow);
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, repo.Events.First().Status);
    }

    // S3: Already active event → remains active (idempotent)
    [Fact]
    public async Task GivenActiveEvent_WhenHandlingActivateEventCommand_ThenRemainsActiveAndSuccess()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        evt.Status = EventStatus.ACTIVE;
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, repo.Events.First().Status);
    }

    // F1 (not found) → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingActivateEventCommand_ThenFailureWithNotFound()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(Guid.NewGuid().ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }

    // F1 (default title) → failure propagated from MakeReady
    [Fact]
    public async Task GivenDraftEventWithDefaultTitle_WhenHandlingActivateEventCommand_ThenFailureWithTitleIsDefault()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TitleIsDefault);
    }

    // F1 (no times) → failure propagated from MakeReady
    [Fact]
    public async Task GivenDraftEventWithNoTimes_WhenHandlingActivateEventCommand_ThenFailureWithTimesNotSet()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TimesNotSet);
    }

    // F2: Cancelled event → failure with CannotModifyCancelled
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingActivateEventCommand_ThenFailureWithCannotModifyCancelled()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        evt.Cancel();
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // Completed event → failure with CannotModifyActive
    [Fact]
    public async Task GivenCompletedEvent_WhenHandlingActivateEventCommand_ThenFailureWithCannotModifyActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ActivateEventCommand> handler = new ActivateEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.COMPLETED;
        await repo.AddAsync(evt);

        var command = ((Success<ActivateEventCommand>)ActivateEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }
}