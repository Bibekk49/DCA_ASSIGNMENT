using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class ActivateEventCommand : ICommand
{
    public EventId EventId { get; }

    private ActivateEventCommand(EventId eventId) => EventId = eventId;

    public static Result<ActivateEventCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultHelper
            .CombineResultsInto<ActivateEventCommand>(idResult)
            .WithPayloadIfSuccess(() => new ActivateEventCommand(idResult.Payload!));
    }
}