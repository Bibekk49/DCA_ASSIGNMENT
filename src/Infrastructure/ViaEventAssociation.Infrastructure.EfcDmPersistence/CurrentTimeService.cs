using DCA_ASSIGNMENT.Core.Domain.Common.Contracts;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public class CurrentTimeService : ICurrentTime
{
    public DateTime GetCurrentTime() => DateTime.Now;
}