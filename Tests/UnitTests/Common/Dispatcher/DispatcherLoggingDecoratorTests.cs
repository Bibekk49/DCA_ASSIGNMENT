using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Common.Dispatcher;

public class DispatcherLoggingDecoratorTests
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

    [Fact]
    public async Task GivenLoggingDecorator_WhenDispatchingUpdateTitle_ThenInnerHandlerIsCalled()
    {
        var mockHandler = new UpdateTitleMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => mockHandler);
        ICommandDispatcher inner = new CommandDispatcher(services.BuildServiceProvider());
        ICommandDispatcher decorator = new LoggingDispatcherDecorator(inner);

        var command = ValidUpdateTitleCommand();
        var result = await decorator.DispatchAsync(command);

        Assert.True(result is Success<None>);
        Assert.True(mockHandler.WasHandled);
    }

    [Fact]
    public async Task GivenLoggingDecorator_WhenDispatchingMakePublic_ThenInnerHandlerIsCalled()
    {
        var mockHandler = new MakePublicMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<MakePublicCommand>>(_ => mockHandler);
        ICommandDispatcher inner = new CommandDispatcher(services.BuildServiceProvider());
        ICommandDispatcher decorator = new LoggingDispatcherDecorator(inner);

        var command = ValidMakePublicCommand();
        var result = await decorator.DispatchAsync(command);

        Assert.True(result is Success<None>);
        Assert.True(mockHandler.WasHandled);
    }

    [Fact]
    public async Task GivenLoggingDecorator_WhenNoHandlerRegistered_ThenExceptionPropagates()
    {
        IServiceCollection services = new ServiceCollection();
        ICommandDispatcher inner = new CommandDispatcher(services.BuildServiceProvider());
        ICommandDispatcher decorator = new LoggingDispatcherDecorator(inner);

        var command = ValidUpdateTitleCommand();

        await Assert.ThrowsAsync<InvalidOperationException>(() => decorator.DispatchAsync(command));
    }

    [Fact]
    public async Task GivenLoggingDecorator_WhenDispatchingUpdateTitle_ThenResultMatchesInnerDispatcher()
    {
        var mockHandler = new UpdateTitleMockHandler();
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<ICommandHandler<UpdateTitleCommand>>(_ => mockHandler);
        IServiceProvider provider = services.BuildServiceProvider();
        ICommandDispatcher inner = new CommandDispatcher(provider);
        ICommandDispatcher decorator = new LoggingDispatcherDecorator(inner);

        var command = ValidUpdateTitleCommand();
        var decoratedResult = await decorator.DispatchAsync(command);
        var directResult = ResultHelper.Success();

        Assert.Equal(decoratedResult.GetType(), directResult.GetType());
    }
}
