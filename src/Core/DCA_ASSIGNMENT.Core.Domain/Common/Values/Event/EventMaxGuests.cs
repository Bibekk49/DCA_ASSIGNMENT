using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelpers;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventMaxGuests : ValueObject
{
    private static readonly int MinGuests = 1;

    public int Value { get; }

    private EventMaxGuests(int maxGuests) => Value = maxGuests;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EventMaxGuests> Create(int maxGuests)
    {
        var validation = Validate(maxGuests);

        if (validation is Failure<None> failure)
            return new Failure<EventMaxGuests>(failure.Errors);

        return new EventMaxGuests(maxGuests);
    }

    private static Result<None> Validate(int maxGuests) =>
        ValidateMinGuests(maxGuests);

    private static Result<None> ValidateMinGuests(int maxGuests) =>
        maxGuests < MinGuests
            ? new ResultError("EventMaxGuests.TooLow", $"Maximum guests must be at least {MinGuests}.", "Validation")
            : Success();
}