using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelpers;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventEndDate : ValueObject
{
    public DateTime Value { get; }

    private EventEndDate(DateTime endDate) => Value = endDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EventEndDate> Create(DateTime endDate)
    {
        var validation = Validate(endDate);

        if (validation is Failure<None> failure)
            return new Failure<EventEndDate>(failure.Errors);

        return new EventEndDate(endDate);
    }

    private static Result<None> Validate(DateTime endDate) =>
        ValidateNotInPast(endDate);

    private static Result<None> ValidateNotInPast(DateTime endDate) =>
        endDate < DateTime.UtcNow
            ? new ResultError("EventEndDate.InPast", "Event end date cannot be in the past.", "Validation")
            : Success();
}