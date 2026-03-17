using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelper;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventTimes 
{
    private static readonly TimeOnly EarliestStart = new(8, 0);
    private static readonly TimeOnly LatestNextDayEnd = new(1, 0);
    private static readonly TimeSpan MinDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(10);

    public DateOnly Date { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    private EventTimes(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<EventTimes> Create(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        var validation = Validate(startTime, endTime);

        if (validation is Failure<None> failure)
            return new Failure<EventTimes>(failure.Errors);

        return new EventTimes(date, startTime, endTime);
    }

    private static Result<None> Validate(TimeOnly start, TimeOnly end) =>
        Combine(
            ValidateStartWindow(start),
            ValidateEndWindow(start, end),
            ValidateDuration(start, end)
        );

    private static Result<None> ValidateDuration(TimeOnly start, TimeOnly end)
    {
        var duration = CalculateDuration(start, end);

        if (duration < MinDuration)
            return EventErrors.Times.DurationTooShort;

        if (duration > MaxDuration)
            return EventErrors.Times.DurationTooLong;

        return Success();
    }

    private static Result<None> ValidateStartWindow(TimeOnly start) =>
        start < EarliestStart
            ? EventErrors.Times.StartTooEarly
            : Success();

    private static Result<None> ValidateEndWindow(TimeOnly startTime, TimeOnly endTime) =>
        endTime <= startTime && endTime > LatestNextDayEnd
            ? EventErrors.Times.EndTooLate
            : Success();

    private static TimeSpan CalculateDuration(TimeOnly startTime, TimeOnly endTime)
    {
        var startSpan = startTime.ToTimeSpan();
        var endSpan = endTime.ToTimeSpan();

        return endTime > startTime
            ? endSpan - startSpan
            : (TimeSpan.FromHours(24) - startSpan) + endSpan;
    }
}