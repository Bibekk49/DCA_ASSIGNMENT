using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateTimesCommand : ICommand
{
    public EventId EventId { get; }
    public EventTimes Times { get; }

    private UpdateTimesCommand(EventId eventId, EventTimes times)
        => (EventId, Times) = (eventId, times);

    public static Result<UpdateTimesCommand> Create(
        string id,
        DateOnly startDate,
        TimeOnly startTime,
        DateOnly endDate,
        TimeOnly endTime)
    {
        var idResult = EventId.FromString(id);
        var timesResult = EventTimes.Create(startDate, startTime, endDate, endTime);

        return ResultHelper
            .CombineResultsInto<UpdateTimesCommand>(idResult, timesResult)
            .WithPayloadIfSuccess(() => new UpdateTimesCommand(idResult.Payload!, timesResult.Payload!));
    }
}
