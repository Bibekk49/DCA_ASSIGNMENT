using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Queries;

public class BrowseUpcomingEventsQueryHandler(ReadDbContext context, ISystemTime currentTime)
    : IQueryHandler<BrowseUpcomingEvents.Query, BrowseUpcomingEvents.Answer>
{
    public async Task<BrowseUpcomingEvents.Answer> HandleAsync(BrowseUpcomingEvents.Query query)
    {
        var now = currentTime.CurrentTime().ToString("yyyy-MM-dd HH:mm:ss");

        var baseQuery = context.Events
            .Where(e => e.Status == "ACTIVE" && e.Visibility == "PUBLIC"
                        && e.StartDate != null
                        && string.Compare(e.StartDate + " " + e.StartTime, now) >= 0);

        if (!string.IsNullOrWhiteSpace(query.TitleSearch))
            baseQuery = baseQuery.Where(e => e.Title.Contains(query.TitleSearch));

        var total = await baseQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)total / query.PageSize);

        var events = await baseQuery
            .OrderBy(e => e.StartDate)
            .ThenBy(e => e.StartTime)
            .Skip(query.PageSize * (query.PageNum - 1))
            .Take(query.PageSize)
            .Select(e => new BrowseUpcomingEvents.EventSummary(
                e.Id,
                e.Title,
                e.Description ?? "",
                e.StartDate!,
                e.StartTime ?? "",
                e.MaxGuestNumber,
                e.Visibility
            ))
            .ToListAsync();

        return new BrowseUpcomingEvents.Answer(events, totalPages);
    }
}
