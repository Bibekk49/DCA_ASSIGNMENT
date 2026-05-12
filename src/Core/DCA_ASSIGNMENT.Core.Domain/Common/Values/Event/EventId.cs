using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
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

    public static Result<EventId> FromString(string id)
    {
        if (!Guid.TryParse(id, out var guid) || guid == Guid.Empty)
            return EventErrors.Id.IdEmpty;
        return Create(guid);
    }

    public static Result<EventId> New()
        => Create(Guid.NewGuid());

    internal static EventId Reconstitute(Guid value) => new(value);

    public override bool Equals(object? obj) => obj is EventId other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(EventId? a, EventId? b) =>
        a is null ? b is null : a.Equals(b);
    public static bool operator !=(EventId? a, EventId? b) => !(a == b);
}