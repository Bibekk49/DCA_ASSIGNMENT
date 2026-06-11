using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelper;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventMaxGuests : ValueObject
{
    private static readonly int MinGuests = 5;
    private static readonly int MaxGuests = 50;

    public int Value { get; }

    private EventMaxGuests(int maxGuests) => Value = maxGuests;

    internal static EventMaxGuests Reconstitute(int value) => new(value);

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
        ResultHelper.Combine(
            ValidateMin(maxGuests),
            ValidateMax(maxGuests)
        );

    private static Result<None> ValidateMin(int maxGuests) =>
        maxGuests < MinGuests
            ? EventErrors.MaxGuests.TooLow
            : Success();

    private static Result<None> ValidateMax(int maxGuests) =>
        maxGuests > MaxGuests
            ? EventErrors.MaxGuests.TooHigh
            : Success();
}