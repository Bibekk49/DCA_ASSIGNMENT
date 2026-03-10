using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
public sealed class EventId
{
    public Guid Value { get; }

    private EventId(Guid eventId) => Value = eventId;

    public static Result<EventId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return new ResultError("event_id.empty", "EventId cannot be empty.", "validation");

        return new EventId(value);
    }

    public static Result<EventId> New()
        => Create(Guid.NewGuid());
}