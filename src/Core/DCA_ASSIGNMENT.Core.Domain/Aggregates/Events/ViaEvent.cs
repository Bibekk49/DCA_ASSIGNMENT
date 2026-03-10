using DCA_ASSIGNMENT.Core.Domain.Common.Bases;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent : IDEntity<EventId>
{
    private ViaEvent(EventId id) : base(id) { }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> f)
            return ResultHelpers.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;
        return ResultHelpers.Success(new ViaEvent(id));
    }
}