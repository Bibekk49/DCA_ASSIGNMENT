using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class MakePublicCommand : ICommand
{
    public EventId EventId { get; }

    private MakePublicCommand(EventId eventId)
        => EventId = eventId;

    public static Result<MakePublicCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultHelper
            .CombineResultsInto<MakePublicCommand>(idResult)
            .WithPayloadIfSuccess(() => new MakePublicCommand(idResult.Payload!));
    }
}
