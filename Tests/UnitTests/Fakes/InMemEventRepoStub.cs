using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

namespace UnitTests.Fakes;

public class InMemEventRepoStub : IEventRepository
{
    public List<ViaEvent> Events { get; } = new();

    public Task AddAsync(ViaEvent viaEvent)
    {
        Events.Add(viaEvent);
        return Task.CompletedTask;
    }

    public Task<ViaEvent?> GetByIdAsync(EventId id)
    {
        var evt = Events.FirstOrDefault(e => e.Id.Value == id.Value);
        return Task.FromResult(evt);
    }
}
