using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Queries;

public class ViewSingleEventQueryHandler(ReadDbContext context)
    : IQueryHandler<ViewSingleEvent.Query, ViewSingleEvent.Answer>
{
    public async Task<ViewSingleEvent.Answer> HandleAsync(ViewSingleEvent.Query query)
    {
        return await context.Events
            .Where(e => e.Id == query.EventId)
            .Select(e => new ViewSingleEvent.Answer(
                e.Id,
                e.Title,
                e.Description ?? "",
                e.Location != null ? e.Location.Name : null,
                e.StartDate,
                e.StartTime,
                e.Visibility,
                e.MaxGuestNumber
            ))
            .SingleAsync();
    }
}