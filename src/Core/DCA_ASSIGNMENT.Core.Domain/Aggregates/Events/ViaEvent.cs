using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent
{
    public EventId Id { get; }
    public EventTitle? Title { get; private set; }

    private ViaEvent(EventId id) : base() { Id = id; }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> f)
            return ResultHelpers.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;
        return ResultHelpers.Success(new ViaEvent(id));
    }

    public Result<None> UpdateTitle(EventTitle newTitle)
    {
        Title = newTitle;
        return ResultHelpers.Success();
    }
}
