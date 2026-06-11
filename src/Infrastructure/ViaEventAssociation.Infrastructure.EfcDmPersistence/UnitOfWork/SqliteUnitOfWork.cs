using DCA_ASSIGNMENT.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.UnitOfWork;

public class SqliteUnitOfWork(EfcDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}