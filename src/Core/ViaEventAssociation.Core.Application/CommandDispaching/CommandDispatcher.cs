using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;

namespace ViaEventAssociation.Core.Application.CommandDispaching;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var serviceType = typeof(ICommandHandler<TCommand>);
        var service = serviceProvider.GetService(serviceType);
        if (service is null)
            throw new InvalidOperationException($"No handler registered for {typeof(TCommand).Name}");
        return ((ICommandHandler<TCommand>)service).HandleAsync(command);
    }
}
