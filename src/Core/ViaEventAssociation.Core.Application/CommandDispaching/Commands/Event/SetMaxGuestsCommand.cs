using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class SetMaxGuestsCommand : ICommand
{
    public EventId EventId { get; }
    public EventMaxGuests MaxGuests { get; }

    private SetMaxGuestsCommand(EventId eventId, EventMaxGuests maxGuests)
        => (EventId, MaxGuests) = (eventId, maxGuests);

    public static Result<SetMaxGuestsCommand> Create(string id, int maxGuests)
    {
        var idResult = EventId.FromString(id);
        var maxGuestsResult = EventMaxGuests.Create(maxGuests);

        return ResultHelper
            .CombineResultsInto<SetMaxGuestsCommand>(idResult, maxGuestsResult)
            .WithPayloadIfSuccess(() => new SetMaxGuestsCommand(idResult.Payload!, maxGuestsResult.Payload!));
    }
}