using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.SetMaxGuests;

public class SetMaxGuestsAggregateUnitTests
{
    // S1: Successfully set max guests on a Draft event (number < 50)
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    public void GivenDraftEventAndValidMaxGuests_WhenSettingMaxGuests_ThenMaxGuestsIsUpdated(int count)
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        EventMaxGuests maxGuests = ((Success<EventMaxGuests>)EventMaxGuests.Create(count)).Value;

        Result<None> result = evt.SetMaxGuests(maxGuests);

        Assert.True(result is Success<None>);
        Assert.Equal(count, evt.MaxGuestNumber.Value);
    }

    // S2: Successfully set max guests on a Ready event (>= 5) — reverts to Draft
    [Theory]
    [InlineData(5)]
    [InlineData(50)]
    public void GivenReadyEvent_WhenSettingMaxGuests_ThenStatusRevertsToD(int count)
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.READY;
        EventMaxGuests maxGuests = ((Success<EventMaxGuests>)EventMaxGuests.Create(count)).Value;

        Result<None> result = evt.SetMaxGuests(maxGuests);

        Assert.True(result is Success<None>);
        Assert.Equal(count, evt.MaxGuestNumber.Value);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    // S3: Active event — increasing max guests is allowed
    [Fact]
    public void GivenActiveEventWithLowerValue_WhenIncreasingMaxGuests_ThenSuccess()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.ACTIVE;
        // Default is 5; increase to 30
        EventMaxGuests maxGuests = ((Success<EventMaxGuests>)EventMaxGuests.Create(30)).Value;

        Result<None> result = evt.SetMaxGuests(maxGuests);

        Assert.True(result is Success<None>);
        Assert.Equal(30, evt.MaxGuestNumber.Value);
    }

    // F1: Active event — reducing max guests is not allowed
    [Fact]
    public void GivenActiveEventWithHigherValue_WhenReducingMaxGuests_ThenFailureWithCannotReduce()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.MaxGuestNumber = ((Success<EventMaxGuests>)EventMaxGuests.Create(30)).Value;
        evt.Status = EventStatus.ACTIVE;
        EventMaxGuests lower = ((Success<EventMaxGuests>)EventMaxGuests.Create(10)).Value;

        Result<None> result = evt.SetMaxGuests(lower);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.MaxGuests.CannotReduceWhenActive);
    }

    // F2: Cannot set max guests on a Cancelled event
    [Fact]
    public void GivenCancelledEvent_WhenSettingMaxGuests_ThenFailureWithCannotModifyCancelled()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Cancel();
        EventMaxGuests maxGuests = ((Success<EventMaxGuests>)EventMaxGuests.Create(10)).Value;

        Result<None> result = evt.SetMaxGuests(maxGuests);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyCancelled);
    }

    // F3: Cannot set max guests on a Completed event
    [Fact]
    public void GivenCompletedEvent_WhenSettingMaxGuests_ThenFailureWithCannotModifyActive()
    {
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        evt.Status = EventStatus.COMPLETED;
        EventMaxGuests maxGuests = ((Success<EventMaxGuests>)EventMaxGuests.Create(10)).Value;

        Result<None> result = evt.SetMaxGuests(maxGuests);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Status.CannotModifyActive);
    }

    // F4: Max guests below minimum (< 5) is rejected by the value object
    [Theory]
    [InlineData(4)]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenMaxGuestsBelowMinimum_WhenCreatingValueObject_ThenFailure(int count)
    {
        Result<EventMaxGuests> result = EventMaxGuests.Create(count);

        Assert.True(result is Failure<EventMaxGuests>);
        Assert.Contains(((Failure<EventMaxGuests>)result).Errors, e => e == EventErrors.MaxGuests.TooLow);
    }

    // F5: Max guests above maximum (> 50) is rejected by the value object
    [Theory]
    [InlineData(51)]
    [InlineData(100)]
    public void GivenMaxGuestsAboveMaximum_WhenCreatingValueObject_ThenFailure(int count)
    {
        Result<EventMaxGuests> result = EventMaxGuests.Create(count);

        Assert.True(result is Failure<EventMaxGuests>);
        Assert.Contains(((Failure<EventMaxGuests>)result).Errors, e => e == EventErrors.MaxGuests.TooHigh);
    }
}