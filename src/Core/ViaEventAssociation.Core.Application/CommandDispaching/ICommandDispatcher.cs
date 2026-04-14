using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;

namespace ViaEventAssociation.Core.Application.CommandDispaching;

public interface ICommandDispatcher
{
    Task<Result> DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
}
