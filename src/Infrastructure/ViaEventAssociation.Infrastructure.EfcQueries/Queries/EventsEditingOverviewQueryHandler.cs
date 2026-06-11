using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Queries;

public class EventsEditingOverviewQueryHandler(ReadDbContext context)
    : IQueryHandler<EventsEditingOverview.Query, EventsEditingOverview.Answer>
{
    public async Task<EventsEditingOverview.Answer> HandleAsync(EventsEditingOverview.Query query)
    {
        var allEvents = await context.Events
            .Where(e => e.Status == "draft" || e.Status == "ready" || e.Status == "cancelled")
            .OrderBy(e => e.Title)
            .Select(e => new { e.Id, e.Title, e.Status })
            .ToListAsync();

        var drafts = allEvents
            .Where(e => e.Status == "draft")
            .Select(e => new EventsEditingOverview.EventInfo(e.Id, e.Title))
            .ToList();

        var ready = allEvents
            .Where(e => e.Status == "ready")
            .Select(e => new EventsEditingOverview.EventInfo(e.Id, e.Title))
            .ToList();

        var cancelled = allEvents
            .Where(e => e.Status == "cancelled")
            .Select(e => new EventsEditingOverview.EventInfo(e.Id, e.Title))
            .ToList();

        return new EventsEditingOverview.Answer(drafts, ready, cancelled);
    }
}
