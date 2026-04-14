using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateDescriptionCommand : ICommand
{
    public EventId EventId { get; }
    public EventDescription Description { get; }

    private UpdateDescriptionCommand(EventId eventId, EventDescription description)
        => (EventId, Description) = (eventId, description);

    public static Result<UpdateDescriptionCommand> Create(string id, string? description)
    {
        var idResult = EventId.FromString(id);
        var descResult = EventDescription.Create(description);

        return ResultHelper
            .CombineResultsInto<UpdateDescriptionCommand>(idResult, descResult)
            .WithPayloadIfSuccess(() => new UpdateDescriptionCommand(idResult.Payload!, descResult.Payload!));
    }
}
