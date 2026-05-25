using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Infrastructure.EfcQueries;
using ViaEventAssociation.Infrastructure.EfcQueries.SeedFactories;

namespace IntegrationTests.Queries;

public abstract class QueryTestBase : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    protected ReadDbContext Context = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ReadDbContext(options);
        await SeedFactory.SeedAsync(Context);
    }

    public Task DisposeAsync()
    {
        Context.Dispose();
        _connection.Dispose();
        return Task.CompletedTask;
    }
}

public class FakeSystemTime(DateTime fixedTime) : ISystemTime
{
    public DateTime CurrentTime() => fixedTime;
}
