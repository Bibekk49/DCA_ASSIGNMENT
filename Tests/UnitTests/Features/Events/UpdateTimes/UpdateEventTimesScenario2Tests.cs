using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.Features.Events.UpdateTimes;

public class UpdateEventTimesScenario2Tests
{
    // S2: Overnight events — start on one day, end on the next day before 01:00
    // 2023/08/25 19:00 → 2023/08/26 01:00  (6h, crosses midnight, end = 01:00 ✅)
    // 2023/08/25 12:00 → 2023/08/25 16:30  (same-day, still valid per S1/S2 shared rules)
    // 2023/08/25 08:00 → 2023/08/25 12:15  (same-day)
    [Theory]
    [InlineData(19, 0, 1, 0)]   // overnight: 19:00 → 01:00 next day (6h)
    [InlineData(12, 0, 16, 30)] // same-day:  12:00 → 16:30
    [InlineData(8,  0, 12, 15)] // same-day:  08:00 → 12:15
    public void GivenDraftEvent_WhenSettingValidOvernightOrSameDayTimes_ThenTimesAreUpdated(
        int startHour, int startMinute, int endHour, int endMinute)
    {
        // Arrange
        ViaEvent evt = ((Success<ViaEvent>)ViaEvent.Create()).Value;

        // For overnight case the dates differ: start = 2023-08-25, end = 2023-08-26
        var startDate = new DateOnly(2023, 8, 25);
        var startTime = new TimeOnly(startHour, startMinute);
        var endTime   = new TimeOnly(endHour, endMinute);

        EventTimes times = ((Success<EventTimes>)EventTimes.Create(startDate, startTime, endTime)).Value;

        // Act
        Result<None> result = evt.UpdateTimes(times);

        // Assert
        Assert.IsType<Success<None>>(result);
        Assert.NotNull(evt.Times);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }
}
