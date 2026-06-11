using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public class SystemTimeService : ISystemTime
{
    public DateTime CurrentTime() => DateTime.Now;
}