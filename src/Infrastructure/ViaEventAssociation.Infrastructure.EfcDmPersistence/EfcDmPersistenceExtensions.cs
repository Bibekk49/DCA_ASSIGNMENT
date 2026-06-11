using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Infrastructure.EfcDmPersistence.UnitOfWork;
using ViaEventAssociation.Infrastructure.EfcDmPersistence.VeaEventPersistence;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public static class EfcDmPersistenceExtensions
{
    public static IServiceCollection AddWritePersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EfcDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IEventRepository, VeaEventEfcRepository>();
        services.AddScoped<DCA_ASSIGNMENT.Core.Domain.Common.IUnitOfWork, SqliteUnitOfWork>();
        services.AddSingleton<ICurrentTime, CurrentTimeService>();
        return services;
    }
}