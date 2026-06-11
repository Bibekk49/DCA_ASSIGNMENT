using DCA_ASSIGNMENT.Core.Domain.Common.Contracts;

namespace UnitTests.Fakes;

public class FakeCurrentTime : ICurrentTime
{
    private readonly DateTime _fixedTime;

    public FakeCurrentTime(DateTime fixedTime) => _fixedTime = fixedTime;

    public DateTime GetCurrentTime() => _fixedTime;
}