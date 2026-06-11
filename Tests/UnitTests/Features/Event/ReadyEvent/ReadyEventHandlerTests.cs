using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.ReadyEvent;

public class ReadyEventHandlerTests
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

    // S1: Valid draft event → status becomes READY
    [Fact]
    public async Task GivenValidDraftEvent_WhenHandlingReadyEventCommand_ThenStatusIsReady()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.READY, repo.Events.First().Status);
    }

    // F1 (event not found) → failure with NotFound error
    [Fact]
    public async Task GivenNonExistentEvent_WhenHandlingReadyEventCommand_ThenFailureWithNotFound()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(Guid.NewGuid().ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }

    // F1 (missing title) → failure with TitleIsDefault
    [Fact]
    public async Task GivenDraftEventWithDefaultTitle_WhenHandlingReadyEventCommand_ThenFailureWithTitleIsDefault()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TitleIsDefault);
    }

    // F1 (missing times) → failure with TimesNotSet
    [Fact]
    public async Task GivenDraftEventWithNoTimes_WhenHandlingReadyEventCommand_ThenFailureWithTimesNotSet()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TimesNotSet);
    }

    // F2: Cancelled event → failure with CannotModifyCancelled
    [Fact]
    public async Task GivenCancelledEvent_WhenHandlingReadyEventCommand_ThenFailureWithCannotModifyCancelled()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        evt.Cancel();
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F3: Start time in past → failure with EventIsInThePast
    [Fact]
    public async Task GivenEventWithStartTimeInPast_WhenHandlingReadyEventCommand_ThenFailureWithEventIsInThePast()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var laterNow = new DateTime(2027, 9, 1, 10, 0, 0); // after event start
        var currentTime = new FakeCurrentTime(laterNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);
        // Event starts 2027-08-25, but "now" is 2027-09-01 → in the past
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.EventIsInThePast);
    }

    // Completed event → failure with CannotModifyActive
    [Fact]
    public async Task GivenCompletedEvent_WhenHandlingReadyEventCommand_ThenFailureWithCannotModifyActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var currentTime = new FakeCurrentTime(FixedNow);
        ICommandHandler<ReadyEventCommand> handler = new ReadyEventHandler(repo, uow, currentTime);

        var evt = ReadyableEvent();
        evt.Status = EventStatus.COMPLETED;
        await repo.AddAsync(evt);

        var command = ((Success<ReadyEventCommand>)ReadyEventCommand.Create(evt.Id.Value.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }
}