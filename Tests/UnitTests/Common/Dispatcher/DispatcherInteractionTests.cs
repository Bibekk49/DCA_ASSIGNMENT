using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Common.Dispatcher;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Common.Dispatcher;

public class DispatcherInteractionTests
{
    private static UpdateTitleCommand ValidUpdateTitleCommand()
    {
        var id = Guid.NewGuid().ToString();
        return ((Success<UpdateTitleCommand>)UpdateTitleCommand.Create(id, "Valid Title")).Value;
    }

    private static MakePublicCommand ValidMakePublicCommand()
    {
        var id = Guid.NewGuid().ToString();
        return ((Success<MakePublicCommand>)MakePublicCommand.Create(id)).Value;
    }

    // Z — Zero handlers registered
    [Fact]
    public async Task GivenNoHandlerRegistered_WhenDispatchingUpdateTitle_ThenThrowsException()
    {
        IServiceCollection services = new ServiceCollection();
        IServiceProvider provider = services.BuildServiceProvider();
        ICommandDispatcher dispatcher = new CommandDispatcher(provider);

        var command = ValidUpdateTitleCommand();

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(command));
    }

    // O — One correct handler (UC2)
    [Fact]
    public async Task GivenUpdateTitleHandlerRegistered_WhenDispatchingUpdateTitle_ThenHandlerIsCalled()
    {
        var mockHandler = new UpdateTitleMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => mockHandler);
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ValidUpdateTitleCommand();
        await dispatcher.DispatchAsync(command);

        Assert.True(mockHandler.WasHandled);
        Assert.Equal(command, mockHandler.HandledCommand);
    }

    // O — One correct handler (UC5)
    [Fact]
    public async Task GivenMakePublicHandlerRegistered_WhenDispatchingMakePublic_ThenHandlerIsCalled()
    {
        var mockHandler = new MakePublicMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<MakePublicCommand>>(_ => mockHandler);
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ValidMakePublicCommand();
        await dispatcher.DispatchAsync(command);

        Assert.True(mockHandler.WasHandled);
        Assert.Equal(command, mockHandler.HandledCommand);
    }

    // O — One incorrect handler registered, wrong command dispatched
    [Fact]
    public async Task GivenUpdateTitleHandlerRegistered_WhenDispatchingMakePublic_ThenThrowsException()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>, UpdateTitleMockHandler>();
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ValidMakePublicCommand();

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(command));
    }

    // M — Many handlers registered, only correct one (UpdateTitle) is called
    [Fact]
    public async Task GivenBothHandlersRegistered_WhenDispatchingUpdateTitle_ThenOnlyUpdateTitleHandlerIsCalled()
    {
        var updateTitleHandler = new UpdateTitleMockHandler();
        var makePublicHandler = new MakePublicMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => updateTitleHandler);
        services.AddScoped<ICommandHandler<MakePublicCommand>>(_ => makePublicHandler);
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ValidUpdateTitleCommand();
        await dispatcher.DispatchAsync(command);

        Assert.True(updateTitleHandler.WasHandled);
        Assert.False(makePublicHandler.WasHandled);
    }

    // M — Many handlers registered, only correct one (MakePublic) is called
    [Fact]
    public async Task GivenBothHandlersRegistered_WhenDispatchingMakePublic_ThenOnlyMakePublicHandlerIsCalled()
    {
        var updateTitleHandler = new UpdateTitleMockHandler();
        var makePublicHandler = new MakePublicMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => updateTitleHandler);
        services.AddScoped<ICommandHandler<MakePublicCommand>>(_ => makePublicHandler);
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ValidMakePublicCommand();
        await dispatcher.DispatchAsync(command);

        Assert.False(updateTitleHandler.WasHandled);
        Assert.True(makePublicHandler.WasHandled);
    }

    // M — Many handlers registered, excluding correct
    [Fact]
    public async Task GivenMultipleHandlersRegistered_WhenDispatchingCreateEvent_ThenThrowsException()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>, UpdateTitleMockHandler>();
        services.AddScoped<ICommandHandler<MakePublicCommand>, MakePublicMockHandler>();
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        var command = ((Success<CreateEventCommand>)CreateEventCommand.Create()).Value;

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(command));
    }

    // S — Same handler called exactly once
    [Fact]
    public async Task GivenHandlerRegistered_WhenDispatchedOnce_ThenHandlerCalledExactlyOnce()
    {
        var mockHandler = new UpdateTitleMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => mockHandler);
        ICommandDispatcher dispatcher = new CommandDispatcher(services.BuildServiceProvider());

        await dispatcher.DispatchAsync(ValidUpdateTitleCommand());

        Assert.True(mockHandler.WasHandled);
        Assert.NotNull(mockHandler.HandledCommand);
    }
}
