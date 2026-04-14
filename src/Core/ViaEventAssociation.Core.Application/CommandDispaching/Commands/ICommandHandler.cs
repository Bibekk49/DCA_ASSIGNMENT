using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands;

internal interface ICommandHandler<ICommand>
{
   Task<Result> HandleAsync(ICommand command);
}