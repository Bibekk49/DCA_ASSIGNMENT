using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateDescriptionCommand : ICommand
{
    public EventId EventId { get; }
    public EventDescription Description { get; }

    private UpdateDescriptionCommand(EventId eventId, EventDescription description)
    {
        EventId = eventId;
        Description = description;
    }

    public static Result<UpdateDescriptionCommand> Create(string eventIdStr, string? description)
    {
        var errors = new List<ResultError>();

        EventId? eventId = null;
        if (!Guid.TryParse(eventIdStr, out var guid) || guid == Guid.Empty)
            errors.Add(EventErrors.Id.IdEmpty);
        else
        {
            var idResult = EventId.Create(guid);
            if (idResult is Failure<EventId> idFail)
                errors.AddRange(idFail.Errors);
            else
                eventId = ((Success<EventId>)idResult).Value;
        }

        EventDescription? eventDescription = null;
        var descResult = EventDescription.Create(description);
        if (descResult is Failure<EventDescription> descFail)
            errors.AddRange(descFail.Errors);
        else
            eventDescription = ((Success<EventDescription>)descResult).Value;

        if (errors.Count > 0)
            return ResultHelper.Failure<UpdateDescriptionCommand>(errors);

        return ResultHelper.Success(new UpdateDescriptionCommand(eventId!, eventDescription!));
    }
}
