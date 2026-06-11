using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.ActivateEvent;

public class ActivateEventAggregateUnitTests
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

    // S1: Draft event with all valid fields → auto-readied then ACTIVE
    [Fact]
    public void GivenValidDraftEvent_WhenActivating_ThenStatusIsActive()
    {
        DomainEvent evt = ReadyableEvent();

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, evt.Status);
    }

    // S2: Ready event → status becomes ACTIVE
    [Fact]
    public void GivenReadyEvent_WhenActivating_ThenStatusIsActive()
    {
        DomainEvent evt = ReadyableEvent();
        evt.MakeReady(FixedNow);

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, evt.Status);
    }

    // S3: Already active event → remains active (idempotent)
    [Fact]
    public void GivenActiveEvent_WhenActivating_ThenRemainsActiveAndSuccess()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Status = EventStatus.ACTIVE;

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Success<None>);
        Assert.Equal(EventStatus.ACTIVE, evt.Status);
    }

    // F1: Draft event with default title → ready check fails → failure with TitleIsDefault
    [Fact]
    public void GivenDraftEventWithDefaultTitle_WhenActivating_ThenFailureWithTitleIsDefault()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.UpdateTimes(ValidFutureTimes(), FixedNow);

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TitleIsDefault);
    }

    // F1 (cont): Draft event with no times → ready check fails → failure with TimesNotSet
    [Fact]
    public void GivenDraftEventWithNoTimes_WhenActivating_ThenFailureWithTimesNotSet()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        var title = ((Success<EventTitle>)EventTitle.Create("VIA Hackathon")).Value;
        evt.UpdateTitle(title);

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Ready.TimesNotSet);
    }

    // F2: Cancelled event → failure with CannotModifyCancelled
    [Fact]
    public void GivenCancelledEvent_WhenActivating_ThenFailureWithCannotModifyCancelled()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Cancel();

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // Completed event → failure with CannotModifyActive
    [Fact]
    public void GivenCompletedEvent_WhenActivating_ThenFailureWithCannotModifyActive()
    {
        DomainEvent evt = ReadyableEvent();
        evt.Status = EventStatus.COMPLETED;

        Result<None> result = evt.Activate(FixedNow);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }
}