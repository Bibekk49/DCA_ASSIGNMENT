using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;

namespace ViaEventAssociation.Core.Application.CommandDispaching;

public class LoggingDispatcherDecorator(ICommandDispatcher next) : ICommandDispatcher
{
    public async Task<Result> DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        Console.WriteLine($"[LOG] Dispatching: {typeof(TCommand).Name}");
        var result = await next.DispatchAsync(command);
        Console.WriteLine($"[LOG] Dispatched: {typeof(TCommand).Name}");
        return result;
    }
}
