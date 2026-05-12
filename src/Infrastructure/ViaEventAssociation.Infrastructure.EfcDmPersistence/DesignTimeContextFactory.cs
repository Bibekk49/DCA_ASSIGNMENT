using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<EfcDbContext>
{
    public EfcDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EfcDbContext>();
        optionsBuilder.UseSqlite(@"Data Source = VEADatabaseProduction.db");
        return new EfcDbContext(optionsBuilder.Options);
    }
}