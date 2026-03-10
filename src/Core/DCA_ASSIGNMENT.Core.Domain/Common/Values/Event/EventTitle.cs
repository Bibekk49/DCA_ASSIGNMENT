using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelpers;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventTitle : ValueObject
{
    private static readonly int MaxLength = 100;

    public string Value { get; }

    private EventTitle(string title) => Value = title;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EventTitle> Create(string title)
    {
        var validation = Validate(title);

        if (validation is Failure<None> failure)
            return new Failure<EventTitle>(failure.Errors);

        return new EventTitle(title);
    }

    private static Result<None> Validate(string title) =>
        Combine(
            ValidateNotEmpty(title),
            ValidateMaxLength(title)
        );

    private static Result<None> ValidateNotEmpty(string title) =>
        string.IsNullOrWhiteSpace(title)
            ? new ResultError("EventTitle.Empty", "Event title cannot be empty.", "Validation")
            : Success();

    private static Result<None> ValidateMaxLength(string title) =>
        !string.IsNullOrWhiteSpace(title) && title.Length > MaxLength
            ? new ResultError("EventTitle.TooLong", $"Event title cannot exceed {MaxLength} characters.", "Validation")
            : Success();
}