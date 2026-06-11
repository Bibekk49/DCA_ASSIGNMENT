using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.ReadyEvent;

public class ReadyEventAggregateUnitTests
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

    // S1: Draft event with title and times set → status becomes READY
    [Fact]
    public void GivenDraftEventWithTitleAndTimes_WhenMakingReady_ThenStatusIsReady()
    {
        DomainEvent evt = ReadyableEvent();

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.READY, evt.Status);
    }

    // F1: Title is still the default "Working Title" → failure
    [Fact]
    public void GivenDraftEventWithDefaultTitle_WhenMakingReady_ThenFailureWithTitleIsDefault()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TitleIsDefault);
    }

    // F1 (cont): Times not set → failure
    [Fact]
    public void GivenDraftEventWithNoTimes_WhenMakingReady_ThenFailureWithTimesNotSet()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TimesNotSet);
    }

    // F2: Cancelled event → failure with CannotModifyCancelled
    [Fact]
    public void GivenCancelledEvent_WhenMakingReady_ThenFailureWithCannotModifyCancelled()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Cancel();

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F3: Start time is in the past → failure with EventIsInThePast
    [Fact]
    public void GivenEventWithStartTimeInPast_WhenMakingReady_ThenFailureWithEventIsInThePast()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);

        // Set times with a fixed "past" now (use a now in future so UpdateTimes passes, then use a later now for MakeReady)
        var pastNow = new DateTime(2026, 1, 1, 10, 0, 0);
        var times = ((Success<EventTimes>)EventTimes.Create(
            new DateOnly(2026, 6, 1),
            new TimeOnly(10, 0),
            new DateOnly(2026, 6, 1),
            new TimeOnly(14, 0))).Value;
        evt.UpdateTimes(times, pastNow);

        // Now check readying with a time AFTER the event start
        var laterNow = new DateTime(2027, 1, 1, 10, 0, 0);
        Result<None> result = evt.MakeReady(laterNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.EventIsInThePast);
    }

    // F4: Event title is default → failure (same as F1, explicit scenario from spec)
    [Fact]
    public void GivenEventWithDefaultTitle_WhenMakingReady_ThenFailureWithTitleIsDefault()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TitleIsDefault);
    }

    // Active event → failure with CannotModifyActive
    [Fact]
    public void GivenActiveEvent_WhenMakingReady_ThenFailureWithCannotModifyActive()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Status = EventStatus.ACTIVE;

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // Completed event → failure with CannotModifyActive
    [Fact]
    public void GivenCompletedEvent_WhenMakingReady_ThenFailureWithCannotModifyActive()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Status = EventStatus.COMPLETED;

        Result<None> result = evt.MakeReady(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }
}