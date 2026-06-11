using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class BrowseUpcomingEvents
{
    public record Query(int PageNum, int PageSize, string? TitleSearch) : IQuery<Answer>;

    public record Answer(
        List<EventSummary> Events,
        int TotalPages
    );

    public record EventSummary(
        string EventId,
        string Title,
        string Description,
        string Date,
        string StartTime,
        int MaxGuests,
        string Visibility
    );
}
