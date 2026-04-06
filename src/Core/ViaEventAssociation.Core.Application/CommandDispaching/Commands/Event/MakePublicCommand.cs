using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class MakePublicCommand : ICommand
{
    public EventId EventId { get; }

    private MakePublicCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakePublicCommand> Create(string eventIdStr)
    {
        if (!Guid.TryParse(eventIdStr, out var guid) || guid == Guid.Empty)
            return ResultHelper.Failure<MakePublicCommand>(EventErrors.Id.IdEmpty);

        var idResult = EventId.Create(guid);
        if (idResult is Failure<EventId> f)
            return ResultHelper.Failure<MakePublicCommand>(f.Errors);

        return ResultHelper.Success(new MakePublicCommand(((Success<EventId>)idResult).Value));
    }
}
