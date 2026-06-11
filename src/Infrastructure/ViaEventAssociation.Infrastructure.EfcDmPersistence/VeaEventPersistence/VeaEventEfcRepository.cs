using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.VeaEventPersistence;

public class VeaEventEfcRepository(EfcDbContext context)
    : RepositoryBase<ViaEvent, EventId>(context), IEventRepository
{
    public override async Task<ViaEvent?> GetAsync(EventId id) =>
        await context.Events
            .SingleOrDefaultAsync(x => x.Id == id);
}