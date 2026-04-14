using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateTimesCommand : ICommand
{
    public EventId EventId { get; }
    public EventTimes Times { get; }

    private UpdateTimesCommand(EventId eventId, EventTimes times)
    {
        EventId = eventId;
        Times = times;
    }

    public static Result<UpdateTimesCommand> Create(
        string eventIdStr,
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
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
            if (idResult is Failure<EventId> idFail)
                errors.AddRange(idFail.Errors);
            else
                eventId = ((Success<EventId>)idResult).Value;
        }

        EventTimes? times = null;
        var timesResult = EventTimes.Create(startDate, startTime, endDate, endTime);
        if (timesResult is Failure<EventTimes> timesFail)
            errors.AddRange(timesFail.Errors);
        else
            times = ((Success<EventTimes>)timesResult).Value;

        if (errors.Count > 0)
            return ResultHelper.Failure<UpdateTimesCommand>(errors);

        return ResultHelper.Success(new UpdateTimesCommand(eventId!, times!));
    }
}