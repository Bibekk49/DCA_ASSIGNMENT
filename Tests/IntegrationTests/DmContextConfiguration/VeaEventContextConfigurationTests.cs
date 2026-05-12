using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.Data.Sqlite;

namespace IntegrationTests.DmContextConfiguration;

public class VeaEventContextConfigurationTests : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EfcDbContext _ctx;

    public VeaEventContextConfigurationTests()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();
        _ctx = new EfcDbContext(
            new DbContextOptionsBuilder<EfcDbContext>().UseSqlite(_conn).Options);
        _ctx.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _conn.Dispose();
    }

    [Fact]
    public void GivenContext_WhenEnsureCreated_ThenSchemaIsValid()
    {
        Assert.NotNull(_ctx.Events);
    }

    [Fact]
    public async Task GivenDraftEvent_WhenSaved_ThenEventTableHasOneRow()
    {
        var evt = MakeDraftEvent();

        _ctx.Events.Add(evt);
        await _ctx.SaveChangesAsync();

        Assert.Equal(1, await _ctx.Events.CountAsync());
    }

    [Fact]
    public async Task GivenEventWithTimes_WhenSaved_ThenTimesColumnsArePopulated()
    {
        var evt = MakeEventWithTimes();

        _ctx.Events.Add(evt);
        await _ctx.SaveChangesAsync();

        var count = await _ctx.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM Events WHERE StartDate IS NOT NULL")
            .FirstAsync();
        Assert.Equal(1, count);
    }

    private static ViaEvent MakeDraftEvent()
    {
        var result = ViaEvent.Create();
        return ((Success<ViaEvent>)result).Value;
    }

    private static ViaEvent MakeEventWithTimes()
    {
        var evt = MakeDraftEvent();
        var timesResult = EventTimes.Create(
            DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            new TimeOnly(10, 0),
            DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            new TimeOnly(14, 0));
        var times = ((Success<EventTimes>)timesResult).Value;
        evt.UpdateTimes(times, DateTime.Now);
        return evt;
    }
}