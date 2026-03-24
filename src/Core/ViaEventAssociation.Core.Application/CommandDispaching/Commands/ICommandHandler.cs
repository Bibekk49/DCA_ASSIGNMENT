using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands;

public interface ICommandHandler<ICommand>
{
    Task<Result> Handle(ICommand command);
}