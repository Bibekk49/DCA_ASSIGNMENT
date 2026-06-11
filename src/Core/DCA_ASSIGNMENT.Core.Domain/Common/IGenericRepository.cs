using DCA_ASSIGNMENT.Core.Domain.Common.Bases;

namespace DCA_ASSIGNMENT.Core.Domain.Common;

public interface IGenericRepository<TAgg, TId>
    where TAgg : EntityBase<TId>
{
    Task<TAgg?> GetAsync(TId id);
    Task RemoveAsync(TId id);
    Task AddAsync(TAgg aggregate);
}