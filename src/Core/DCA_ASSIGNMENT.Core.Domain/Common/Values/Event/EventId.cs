using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

public sealed class EventId : ValueObject
{
    public Guid Value { get; }

    private EventId(Guid value) => Value = value;

    public static Result<EventId> New() => new Success<EventId>(new EventId(Guid.NewGuid()));

    public static EventId FromGuid(Guid id) => new(id);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
