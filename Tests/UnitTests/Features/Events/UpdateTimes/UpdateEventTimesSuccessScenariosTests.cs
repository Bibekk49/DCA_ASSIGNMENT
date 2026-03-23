using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using EventAggregate = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Events.UpdateTimes;

public class UpdateEventTimesSuccessScenariosTests
{
    private static readonly DateTime FixedNow = new(2027, 1, 1, 10, 0, 0);

    public static IEnumerable<object[]> S1ValidSameDayTimes =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(19, 0), new DateOnly(2027, 8, 25), new TimeOnly(23, 59) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(12, 0), new DateOnly(2027, 8, 25), new TimeOnly(16, 30) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(8, 0),  new DateOnly(2027, 8, 25), new TimeOnly(12, 15) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(10, 0), new DateOnly(2027, 8, 25), new TimeOnly(20, 0) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(13, 0), new DateOnly(2027, 8, 25), new TimeOnly(23, 0) }
        };

    public static IEnumerable<object[]> S2ValidSameDayOrNextDayTimes =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(19, 0), new DateOnly(2027, 8, 26), new TimeOnly(1, 0) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(12, 0), new DateOnly(2027, 8, 25), new TimeOnly(16, 30) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(8, 0),  new DateOnly(2027, 8, 25), new TimeOnly(12, 15) }
        };

    public static IEnumerable<object[]> S3ValidTimes =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(19, 0), new DateOnly(2027, 8, 25), new TimeOnly(23, 59) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(19, 0), new DateOnly(2027, 8, 26), new TimeOnly(1, 0) }
        };

    public static IEnumerable<object[]> S5DurationsTenHoursOrLess =>
        new[]
        {
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(13, 0), new DateOnly(2027, 8, 25), new TimeOnly(23, 0) },
            new object[] { new DateOnly(2027, 8, 25), new TimeOnly(15, 0), new DateOnly(2027, 8, 26), new TimeOnly(1, 0) }
        };

    [Theory]
    [MemberData(nameof(S1ValidSameDayTimes))]
    public void S1_GivenDraftEvent_WhenSettingValidSameDayTimes_ThenTimesAreUpdated(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var evt = CreateEvent();
        var times = CreateTimes(startDate, startTime, endDate, endTime);

        var result = evt.UpdateTimes(times, FixedNow);

        Assert.IsType<Success<None>>(result);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    [Theory]
    [MemberData(nameof(S2ValidSameDayOrNextDayTimes))]
    public void S2_GivenDraftEvent_WhenSettingValidSameDayOrNextDayTimes_ThenTimesAreUpdated(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var evt = CreateEvent();
        var times = CreateTimes(startDate, startTime, endDate, endTime);

        var result = evt.UpdateTimes(times, FixedNow);

        Assert.IsType<Success<None>>(result);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    [Theory]
    [MemberData(nameof(S3ValidTimes))]
    public void S3_GivenReadyEvent_WhenSettingValidTimes_ThenTimesAreUpdatedAndStatusBecomesDraft(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var evt = CreateEvent();
        evt.SetStatusForTesting(EventStatus.READY);

        var times = CreateTimes(startDate, startTime, endDate, endTime);

        var result = evt.UpdateTimes(times, FixedNow);

        Assert.IsType<Success<None>>(result);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    [Fact]
    public void S4_GivenEvent_WhenSettingFutureValidTimes_ThenTimesAreUpdated()
    {
        var evt = CreateEvent();
        var times = CreateTimes(
            new DateOnly(2027, 12, 24),
            new TimeOnly(14, 0),
            new DateOnly(2027, 12, 24),
            new TimeOnly(20, 0));

        var result = evt.UpdateTimes(times, FixedNow);

        Assert.IsType<Success<None>>(result);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    [Theory]
    [MemberData(nameof(S5DurationsTenHoursOrLess))]
    public void S5_GivenEvent_WhenDurationIsTenHoursOrLess_ThenTimesAreUpdated(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var evt = CreateEvent();
        var times = CreateTimes(startDate, startTime, endDate, endTime);

        var result = evt.UpdateTimes(times, FixedNow);

        Assert.IsType<Success<None>>(result);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }

    private static EventAggregate CreateEvent()
    {
        var result = EventAggregate.Create();
        return Assert.IsType<Success<EventAggregate>>(result).Value;
    }

    private static EventTimes CreateTimes(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var result = EventTimes.Create(startDate, startTime, endDate, endTime);
        return Assert.IsType<Success<EventTimes>>(result).Value;
    }
}