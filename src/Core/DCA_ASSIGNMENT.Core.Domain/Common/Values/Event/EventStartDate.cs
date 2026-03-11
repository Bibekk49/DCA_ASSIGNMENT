using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelper;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventStartDate : ValueObject
{
    public DateTime Value { get; }

    private EventStartDate(DateTime startDate) => Value = startDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EventStartDate> Create(DateTime startDate)
    {
        var validation = Validate(startDate);

        if (validation is Failure<None> failure)
            return new Failure<EventStartDate>(failure.Errors);

        return new EventStartDate(startDate);
    }

    private static Result<None> Validate(DateTime startDate) =>
        ValidateNotInPast(startDate);

    private static Result<None> ValidateNotInPast(DateTime startDate) =>
        startDate < DateTime.UtcNow
            ? new ResultError("EventStartDate.InPast", "Event start date cannot be in the past.", "Validation")
            : Success();
}