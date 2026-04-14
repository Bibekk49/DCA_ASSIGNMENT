using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;

namespace UnitTests.Common.Dispatcher;

public class CommandHandlerAutoRegistrationTests
{
    private static readonly System.Reflection.Assembly AppAssembly =
        typeof(CreateEventHandler).Assembly;

    [Fact]
    public void GivenApplicationAssembly_WhenAutoRegistering_ThenAllHandlersAreRegistered()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCommandHandlers(AppAssembly);

        Assert.Contains(services, d => d.ServiceType == typeof(ICommandHandler<CreateEventCommand>));
        Assert.Contains(services, d => d.ServiceType == typeof(ICommandHandler<UpdateTitleCommand>));
        Assert.Contains(services, d => d.ServiceType == typeof(ICommandHandler<MakePublicCommand>));
    }

    [Fact]
    public void GivenApplicationAssembly_WhenAutoRegistering_ThenHandlerTypesAreCorrect()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCommandHandlers(AppAssembly);

        var descriptor = services.First(d => d.ServiceType == typeof(ICommandHandler<UpdateTitleCommand>));

        Assert.Equal(typeof(UpdateTitleHandler), descriptor.ImplementationType);
    }

    [Fact]
    public void GivenApplicationAssembly_WhenAutoRegistering_ThenCorrectNumberOfHandlersRegistered()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCommandHandlers(AppAssembly);

        var handlerCount = services.Count(d =>
            d.ServiceType.IsGenericType &&
            d.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

        Assert.Equal(6, handlerCount); // UC1–UC6
    }
}
