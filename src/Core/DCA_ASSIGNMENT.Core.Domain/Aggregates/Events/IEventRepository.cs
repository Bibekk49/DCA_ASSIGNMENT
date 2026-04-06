using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public interface IEventRepository
{
    Task AddAsync(ViaEvent viaEvent);
    Task<ViaEvent?> GetByIdAsync(EventId id);
}
