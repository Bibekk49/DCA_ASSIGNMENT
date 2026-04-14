using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class MakePrivateCommand : ICommand
{
    public EventId EventId { get; }

    private MakePrivateCommand(EventId eventId)
        => EventId = eventId;

    public static Result<MakePrivateCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultHelper
            .CombineResultsInto<MakePrivateCommand>(idResult)
            .WithPayloadIfSuccess(() => new MakePrivateCommand(idResult.Payload!));
    }
}
