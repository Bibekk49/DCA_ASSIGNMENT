using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Infrastructure.EfcQueries.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public static class QueryDbExtensions
{
    public static IServiceCollection AddQueryPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ReadDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ISystemTime, SystemTimeService>();
        services.AddScoped<IQueryHandler<BrowseUpcomingEvents.Query, BrowseUpcomingEvents.Answer>, BrowseUpcomingEventsQueryHandler>();
        services.AddScoped<IQueryHandler<ViewSingleEvent.Query, ViewSingleEvent.Answer>, ViewSingleEventQueryHandler>();
        services.AddScoped<IQueryHandler<EventsEditingOverview.Query, EventsEditingOverview.Answer>, EventsEditingOverviewQueryHandler>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }
}
