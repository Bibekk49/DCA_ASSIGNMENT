using DCA_ASSIGNMENT.Core.Domain.Common;
using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public abstract class RepositoryBase<TAgg, TId>(DbContext context) : IGenericRepository<TAgg, TId>
    where TAgg : EntityBase<TId>
{
    protected readonly DbContext Context = context;

    public virtual async Task<TAgg?> GetAsync(TId id) =>
        await Context.Set<TAgg>().FindAsync(id);

    public virtual async Task RemoveAsync(TId id)
    {
        var entity = await GetAsync(id);
        if (entity is not null)
            Context.Set<TAgg>().Remove(entity);
    }

    public virtual async Task AddAsync(TAgg aggregate) =>
        await Context.Set<TAgg>().AddAsync(aggregate);
}