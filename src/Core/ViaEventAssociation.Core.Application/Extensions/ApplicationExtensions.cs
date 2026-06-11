using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.CommandDispaching;

namespace ViaEventAssociation.Core.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddCommandDispatcher(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddCommandHandlers(typeof(ApplicationExtensions).Assembly);
        return services;
    }
}