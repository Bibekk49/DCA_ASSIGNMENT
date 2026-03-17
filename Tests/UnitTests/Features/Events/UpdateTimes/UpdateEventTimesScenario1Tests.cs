using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using EventAggregate = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Events.UpdateTimes;

public class UpdateEventTimesScenario1Tests
{
    [Theory]
    [InlineData(19, 0, 23, 59)]
    [InlineData(12, 0, 16, 30)]
    [InlineData(8, 0, 12, 15)]
    [InlineData(10, 0, 20, 0)]
    [InlineData(13, 0, 23, 0)]

    [Fact]
    public void GivenDraftEvent_WhenSettingValidTimes_ThenTimesAreUpdated(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        // Arrange
        Result<EventId> eventIdResult = EventId.New().Value;

        Result<ViaEvent> event = ViaEvent.Create(eventIdResult).Value;
        var Date = new DateOnly(2023, 8, 25);
        var startTime = new TimeOnly(startHour, startMinute);
        var endTime = new TimeOnly(endHour, endMinute);

        Result<EventTimes> timesResult = EventTimes.Create(Date, startTime, endTime);

        // Act
        Result<None> result = evt.UpdateTimes(times);

        // Assert
        Assert.IsType<Success<None>>(result);
        Assert.NotNull(evt.Times);
        Assert.Equal(times, evt.Times);
        Assert.Equal(EventStatus.DRAFT, evt.Status);
    }
}
