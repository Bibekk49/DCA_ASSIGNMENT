using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelper;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventTimes
{
    private static readonly TimeOnly EarliestAllowedStartTime = new(8, 0);
    private static readonly TimeOnly LatestAllowedNextDayEndTime = new(1, 0);
    private static readonly TimeSpan MinimumAllowedDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan MaximumAllowedDuration = TimeSpan.FromHours(10);

    public DateOnly StartDate { get; }
    public TimeOnly StartTime { get; }
    public DateOnly EndDate { get; }
    public TimeOnly EndTime { get; }

    private EventTimes(DateOnly startDate, TimeOnly startTime, DateOnly endDate, TimeOnly endTime)
    {
        StartDate = startDate;
        StartTime = startTime;
        EndDate = endDate;
        EndTime = endTime;
    }

    public static Result<EventTimes> Create(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var validation = ValidateBusinessRules(startDate, startTime, endDate, endTime);

        if (validation is Failure<None> failure)
            return new Failure<EventTimes>(failure.Errors);

        return new EventTimes(startDate, startTime, endDate, endTime);
    }

    private static Result<None> ValidateBusinessRules(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime) =>
        Combine(
            ValidateStartDateIsNotAfterEndDate(startDate, endDate),
            ValidateStartTimeIsNotBefore08_00(startTime),
            ValidateDateRangeAndEndTimeRules(startDate, startTime, endDate, endTime),
            ValidateDurationIsBetween1And10Hours(startDate, startTime, endDate, endTime)
        );

    private static Result<None> ValidateStartDateIsNotAfterEndDate(DateOnly startDate, DateOnly endDate) =>
        startDate > endDate
            ? EventErrors.Times.StartDateMustBeOnOrBeforeEndDate
            : Success();

    private static Result<None> ValidateStartTimeIsNotBefore08_00(TimeOnly startTime) =>
        startTime < EarliestAllowedStartTime
            ? EventErrors.Times.StartTooEarly
            : Success();

    private static Result<None> ValidateDateRangeAndEndTimeRules(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        if (startDate == endDate)
            return ValidateSameDayTimeRange(startTime, endTime);

        if (startDate.AddDays(1) == endDate)
            return ValidateNextDayEndingTime(endTime);

        return EventErrors.Times.StartMustBeBeforeEnd;
    }

    private static Result<None> ValidateSameDayTimeRange(TimeOnly startTime, TimeOnly endTime) =>
        startTime >= endTime
            ? EventErrors.Times.StartTimeMustBeBeforeEndTime
            : Success();

    private static Result<None> ValidateNextDayEndingTime(TimeOnly endTime) =>
        endTime > LatestAllowedNextDayEndTime
            ? EventErrors.Times.EndTooLate
            : Success();

    private static Result<None> ValidateDurationIsBetween1And10Hours(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var duration = CalculateDuration(startDate, startTime, endDate, endTime);

        if (duration < MinimumAllowedDuration)
            return EventErrors.Times.DurationTooShort;

        if (duration > MaximumAllowedDuration)
            return EventErrors.Times.DurationTooLong;

        return Success();
    }

    private static TimeSpan CalculateDuration(
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var startDateTime = startDate.ToDateTime(startTime);
        var endDateTime = endDate.ToDateTime(endTime);

        return endDateTime - startDateTime;
    }
}