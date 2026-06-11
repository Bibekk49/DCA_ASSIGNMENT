using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class ViewSingleEvent
{
    public record Query(string EventId) : IQuery<Answer>;

    public record Answer(
        string EventId,
        string Title,
        string Description,
        string? LocationName,
        string? Date,
        string? StartTime,
        string Visibility,
        int MaxGuests
    );
}
