using ViaEventAssociation.Infrastructure.EfcQueries;
using ViaEventAssociation.Infrastructure.EfcQueries.SeedFactories;

namespace IntegrationTests.Queries;

public class SeedingTest : QueryTestBase
{
    [Fact]
    public async Task Database_Can_Be_Seeded_With_Events()
    {
        var count = await Context.Events.CountAsync();
        Assert.True(count > 0);
    }

    [Fact]
    public async Task Database_Can_Be_Seeded_With_Locations()
    {
        var count = await Context.Locations.CountAsync();
        Assert.True(count > 0);
    }
}
