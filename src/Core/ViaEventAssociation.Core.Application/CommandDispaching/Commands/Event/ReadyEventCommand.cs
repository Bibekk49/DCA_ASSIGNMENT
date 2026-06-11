using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class ReadyEventCommand : ICommand
{
    public EventId EventId { get; }

    private ReadyEventCommand(EventId eventId) => EventId = eventId;

    public static Result<ReadyEventCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultHelper
            .CombineResultsInto<ReadyEventCommand>(idResult)
            .WithPayloadIfSuccess(() => new ReadyEventCommand(idResult.Payload!));
    }
}