using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class MakePrivateCommand : ICommand
{
    public EventId EventId { get; }

    private MakePrivateCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakePrivateCommand> Create(string eventIdStr)
    {
        if (!Guid.TryParse(eventIdStr, out var guid) || guid == Guid.Empty)
            return ResultHelper.Failure<MakePrivateCommand>(EventErrors.Id.IdEmpty);

        var idResult = EventId.Create(guid);
        if (idResult is Failure<EventId> f)
            return ResultHelper.Failure<MakePrivateCommand>(f.Errors);

        return ResultHelper.Success(new MakePrivateCommand(((Success<EventId>)idResult).Value));
    }
}
