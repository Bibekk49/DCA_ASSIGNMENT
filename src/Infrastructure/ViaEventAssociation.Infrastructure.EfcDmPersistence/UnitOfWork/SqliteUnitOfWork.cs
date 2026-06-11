using DCA_ASSIGNMENT.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.UnitOfWork;

public class SqliteUnitOfWork(DbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}