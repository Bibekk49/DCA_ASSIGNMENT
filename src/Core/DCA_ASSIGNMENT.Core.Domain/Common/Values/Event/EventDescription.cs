using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelpers;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventDescription : ValueObject
{
    private static readonly int MaxLength = 1000;

    public string Value { get; }

    private EventDescription(string description) => Value = description;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EventDescription> Create(string description)
    {
        var validation = Validate(description);

        if (validation is Failure<None> failure)
            return new Failure<EventDescription>(failure.Errors);

        return new EventDescription(description);
    }

    private static Result<None> Validate(string description) =>
        Combine(
            ValidateNotEmpty(description),
            ValidateMaxLength(description)
        );

    private static Result<None> ValidateNotEmpty(string description) =>
        string.IsNullOrWhiteSpace(description)
            ? EventErrors.Description.DescriptionEmpty
            : Success();

    private static Result<None> ValidateMaxLength(string description) =>
        !string.IsNullOrWhiteSpace(description) && description.Length > MaxLength
            ? EventErrors.Description.DescriptionTooLong
            : Success();
}

