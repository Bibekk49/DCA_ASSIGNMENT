using DCA_ASSIGNMENT.Core.Domain.Common;

namespace UnitTests.Fakes;

public class FakeUoW : IUnitOfWork
{
    public Task SaveChangesAsync() => Task.CompletedTask;
}
