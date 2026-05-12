using DCA_ASSIGNMENT.Core.Domain.Common;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public interface IEventRepository : IGenericRepository<ViaEvent, EventId>;