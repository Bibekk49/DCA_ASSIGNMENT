using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

namespace IntegrationTests.WebApi;

public class FaultingEventRepository : IEventRepository
{
    public Task<ViaEvent?> GetAsync(EventId id) =>
        throw new InvalidOperationException("Simulated database failure");

    public Task RemoveAsync(EventId id) => Task.CompletedTask;

    public Task AddAsync(ViaEvent aggregate) => Task.CompletedTask;
}
