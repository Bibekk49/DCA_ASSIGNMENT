using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateTitleCommand : ICommand
{
    public EventId EventId { get; }
    public EventTitle Title { get; }

    private UpdateTitleCommand(EventId eventId, EventTitle title)
    {
        EventId = eventId;
        Title = title;
    }

    public static Result<UpdateTitleCommand> Create(string eventIdStr, string title)
    {
        var errors = new List<ResultError>();

        EventId? eventId = null;
        if (!Guid.TryParse(eventIdStr, out var guid) || guid == Guid.Empty)
        {
            errors.Add(EventErrors.Id.IdEmpty);
        }
        else
        {
            var idResult = EventId.Create(guid);
            if (idResult is Failure<EventId> idFail) errors.AddRange(idFail.Errors);
            else eventId = ((Success<EventId>)idResult).Value;
        }

        EventTitle? eventTitle = null;
        var titleResult = EventTitle.Create(title);
        if (titleResult is Failure<EventTitle> titleFail) errors.AddRange(titleFail.Errors);
        else eventTitle = ((Success<EventTitle>)titleResult).Value;

        if (errors.Count > 0)
            return ResultHelper.Failure<UpdateTitleCommand>(errors);

        return ResultHelper.Success(new UpdateTitleCommand(eventId!, eventTitle!));
    }
}
