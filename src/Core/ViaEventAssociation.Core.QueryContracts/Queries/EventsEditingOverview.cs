using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class EventsEditingOverview
{
    public record Query() : IQuery<Answer>;

    public record Answer(
        List<EventInfo> DraftEvents,
        List<EventInfo> ReadyEvents,
        List<EventInfo> CancelledEvents
    );

    public record EventInfo(
        string EventId,
        string Title
    );
}