using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using EventAggregate = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.UpdateTimes;

public class UpdateEventTimesFailureScenariosTests
{
    private static readonly DateTime FixedNow = new(2027, 1, 1, 10, 0, 0);

    public static IEnumerable<object[]> F1StartDateAfterEndDate =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(19, 0), new DateOnly(2027, 8, 25), new TimeOnly(1, 0) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(19, 0), new DateOnly(2027, 8, 25), new TimeOnly(23, 59) },
            new object[] { new DateOnly(2027, 8, 27), new TimeOnly(12, 0), new DateOnly(2027, 8, 25), new TimeOnly(16, 30) },
            new object[] { new DateOnly(2027, 8, 1),  new TimeOnly(8, 0),  new DateOnly(2027, 7, 31), new TimeOnly(12, 15) }
        };

    public static IEnumerable<object[]> F2SameDayStartAfterEnd =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(19, 0), new DateOnly(2027, 8, 26), new TimeOnly(14, 0) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(16, 0), new DateOnly(2027, 8, 26), new TimeOnly(0, 0) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(19, 0), new DateOnly(2027, 8, 26), new TimeOnly(18, 59) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(12, 0), new DateOnly(2027, 8, 26), new TimeOnly(10, 10) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(8, 0),  new DateOnly(2027, 8, 26), new TimeOnly(0, 30) }
        };

    public static IEnumerable<object[]> F3SameDayDurationLessThanOneHour =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(14, 0), new DateOnly(2027, 8, 26), new TimeOnly(14, 50) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(18, 0), new DateOnly(2027, 8, 26), new TimeOnly(18, 59) },
            new object[] { new DateOnly(2027, 8, 26), new TimeOnly(12, 0), new DateOnly(2027, 8, 26), new TimeOnly(12, 30) }
        };

    public static IEnumerable<object[]> F4OvernightDurationLessThanOneHour =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(23, 30), new DateOnly(2027, 8, 26), new TimeOnly(0, 15) },
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(23, 1),  new DateOnly(2027, 8, 31), new TimeOnly(0, 0) },
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(23, 59), new DateOnly(2027, 8, 31), new TimeOnly(0, 1) }
        };

    public static IEnumerable<object[]> F5StartBefore08 =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(7, 50), new DateOnly(2027, 8, 25), new TimeOnly(14, 0) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(7, 59), new DateOnly(2027, 8, 25), new TimeOnly(15, 0) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(1, 1),  new DateOnly(2027, 8, 25), new TimeOnly(8, 30) }
        };

    public static IEnumerable<object[]> F6NextDayEndAfter01 =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 24), new TimeOnly(23, 50), new DateOnly(2027, 8, 25), new TimeOnly(1, 1) },
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(23, 0),  new DateOnly(2027, 8, 31), new TimeOnly(2, 30) }
        };

    public static IEnumerable<object[]> F9DurationMoreThanTenHours =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(8, 0),  new DateOnly(2027, 8, 30), new TimeOnly(18, 1) },
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(14, 59), new DateOnly(2027, 8, 31), new TimeOnly(1, 0) },
            new object[] { new DateOnly(2027, 8, 30), new TimeOnly(14, 0),  new DateOnly(2027, 8, 31), new TimeOnly(0, 1) }
        };

    public static IEnumerable<object[]> F11InvalidClosedHoursCases =>
        new[]
        {
            // Fails with StartTooEarly
            new object[]
            {
                new DateOnly(2027, 8, 31), new TimeOnly(0, 30),
                new DateOnly(2027, 8, 31), new TimeOnly(8, 30),
                EventErrors.Times.StartTooEarly.Code,
                EventErrors.Times.StartTooEarly.Message
            },
            // Fails with EndTooLate
            new object[]
            {
                new DateOnly(2027, 8, 30), new TimeOnly(23, 59),
                new DateOnly(2027, 8, 31), new TimeOnly(8, 1),
                EventErrors.Times.EndTooLate.Code,
                EventErrors.Times.EndTooLate.Message
            },
            // Fails with StartTooEarly
            new object[]
            {
                new DateOnly(2027, 8, 31), new TimeOnly(1, 0),
                new DateOnly(2027, 8, 31), new TimeOnly(8, 0),
                EventErrors.Times.StartTooEarly.Code,
                EventErrors.Times.StartTooEarly.Message
            }
        };

    [Theory]
    [MemberData(nameof(F1StartDateAfterEndDate))]
    public void F1_GivenStartDateAfterEndDate_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.StartDateMustBeOnOrBeforeEndDate.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.StartDateMustBeOnOrBeforeEndDate.Message);
    }

    [Theory]
    [MemberData(nameof(F2SameDayStartAfterEnd))]
    public void F2_GivenSameDateAndStartTimeAfterEndTime_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.StartTimeMustBeBeforeEndTime.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.StartTimeMustBeBeforeEndTime.Message);
    }

    [Theory]
    [MemberData(nameof(F3SameDayDurationLessThanOneHour))]
    public void F3_GivenSameDateAndDurationLessThanOneHour_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.DurationTooShort.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.DurationTooShort.Message);
    }

    [Theory]
    [MemberData(nameof(F4OvernightDurationLessThanOneHour))]
    public void F4_GivenOvernightDurationLessThanOneHour_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.DurationTooShort.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.DurationTooShort.Message);
    }

    [Theory]
    [MemberData(nameof(F5StartBefore08))]
    public void F5_GivenStartTimeBefore08_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.StartTooEarly.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.StartTooEarly.Message);
    }

    [Theory]
    [MemberData(nameof(F6NextDayEndAfter01))]
    public void F6_GivenNextDayEndTimeAfter01_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.EndTooLate.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.EndTooLate.Message);
    }

    [Fact]
    public void F7_GivenActiveEvent_WhenUpdatingTimes_ThenFailureWithCorrectMessage()
    {
        var evt = CreateEvent();
        evt.Status = EventStatus.ACTIVE;

        var timesResult = EventTimes.Create(
            new DateOnly(2027, 12, 24),
            new TimeOnly(14, 0),
            new DateOnly(2027, 12, 24),
            new TimeOnly(20, 0));

        var times = Assert.IsType<Success<EventTimes>>(timesResult).Value;

        var result = evt.UpdateTimes(times, FixedNow);

        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Status.CannotModifyActive.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Status.CannotModifyActive.Message);
    }

    [Fact]
    public void F8_GivenCancelledEvent_WhenUpdatingTimes_ThenFailureWithCorrectMessage()
    {
        var evt = CreateEvent();
        evt.Cancel();

        var timesResult = EventTimes.Create(
            new DateOnly(2027, 12, 24),
            new TimeOnly(14, 0),
            new DateOnly(2027, 12, 24),
            new TimeOnly(20, 0));

        var times = Assert.IsType<Success<EventTimes>>(timesResult).Value;

        var result = evt.UpdateTimes(times, FixedNow);

        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Status.CannotModifyCancelled.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Status.CannotModifyCancelled.Message);
    }

    [Theory]
    [MemberData(nameof(F9DurationMoreThanTenHours))]
    public void F9_GivenDurationMoreThanTenHours_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.DurationTooLong.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.DurationTooLong.Message);
    }

    [Fact]
    public void F10_GivenStartTimeInThePast_WhenUpdatingTimes_ThenFailureWithCorrectMessage()
    {
        var evt = CreateEvent();

        var timesResult = EventTimes.Create(
            new DateOnly(2026, 12, 24),
            new TimeOnly(14, 0),
            new DateOnly(2026, 12, 24),
            new TimeOnly(20, 0));

        var times = Assert.IsType<Success<EventTimes>>(timesResult).Value;

        var result = evt.UpdateTimes(times, FixedNow);

        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e.Code == EventErrors.Times.StartMustBeInFuture.Code);
        Assert.Contains(failure.Errors, e => e.Message == EventErrors.Times.StartMustBeInFuture.Message);
    }

    [Theory]
    [MemberData(nameof(F11InvalidClosedHoursCases))]
    public void F11_GivenTimesThatSpanClosedHours_WhenCreatingEventTimes_ThenFailureWithCorrectMessage(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime,
        string expectedCode,
        string expectedMessage)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);

        var failure = Assert.IsType<Failure<EventTimes>>(result);
        Assert.Contains(failure.Errors, e => e.Code == expectedCode);
        Assert.Contains(failure.Errors, e => e.Message == expectedMessage);
    }

    private static EventAggregate CreateEvent()
    {
        var result = EventAggregate.Create();
        return Assert.IsType<Success<EventAggregate>>(result).Value;
    }
}