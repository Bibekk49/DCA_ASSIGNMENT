using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class UpdateTitleCommand : ICommand
{
    public EventId EventId { get; }
    public EventTitle Title { get; }

    private UpdateTitleCommand(EventId eventId, EventTitle title)
        => (EventId, Title) = (eventId, title);

    public static Result<UpdateTitleCommand> Create(string id, string title)
    {
        var idResult = EventId.FromString(id);
        var titleResult = EventTitle.Create(title);

        return ResultHelper
            .CombineResultsInto<UpdateTitleCommand>(idResult, titleResult)
            .WithPayloadIfSuccess(() => new UpdateTitleCommand(idResult.Payload!, titleResult.Payload!));
    }
}
