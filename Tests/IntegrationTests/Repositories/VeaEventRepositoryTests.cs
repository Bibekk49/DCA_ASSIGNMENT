using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.Data.Sqlite;

namespace IntegrationTests.Repositories;

public class VeaEventRepositoryTests : IDisposable
{
    private readonly SqliteConnection _conn;

    public VeaEventRepositoryTests()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();
        using var setupCtx = MakeContext();
        setupCtx.Database.EnsureCreated();
    }

    public void Dispose() => _conn.Dispose();

    // Both save and load contexts share the same open connection.
    // Using separate context instances clears the EFC identity cache.
    private EfcDbContext MakeContext() =>
        new(new DbContextOptionsBuilder<EfcDbContext>().UseSqlite(_conn).Options);

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
            new TimeOnly(9, 0),
            DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            new TimeOnly(11, 0));
        var times = ((Success<EventTimes>)timesResult).Value;
        evt.UpdateTimes(times, DateTime.Now);
        return evt;
    }

    // ── SAVE + LOAD ────────────────────────────────────────────────────────

    [Fact]
    public async Task GivenDraftEvent_WhenSavedAndLoaded_ThenAllFieldsMatch()
    {
        var evt = MakeDraftEvent();
        var savedId = evt.Id;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.NotNull(loaded);
        Assert.Equal(savedId, loaded.Id);
        Assert.Equal(EventStatus.DRAFT, loaded.Status);
        Assert.Equal(EventVisibility.PRIVATE, loaded.EventVisibility);
        Assert.Equal("Working Title", loaded.Title.Value);
        Assert.Equal("", loaded.Description.Value);
        Assert.Equal(5, loaded.MaxGuestNumber.Value);
        Assert.Null(loaded.Times);
    }

    [Fact]
    public async Task GivenEventWithTimes_WhenSavedAndLoaded_ThenTimesMatch()
    {
        var evt = MakeEventWithTimes();
        var savedId = evt.Id;
        var expectedStart = evt.Times!.StartDate;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Times);
        Assert.Equal(expectedStart, loaded.Times.StartDate);
        Assert.Equal(new TimeOnly(9, 0), loaded.Times.StartTime);
        Assert.Equal(new TimeOnly(11, 0), loaded.Times.EndTime);
    }

    [Fact]
    public async Task GivenCancelledEvent_WhenSavedAndLoaded_ThenStatusIsCancelled()
    {
        var evt = MakeDraftEvent();
        evt.Cancel();
        var savedId = evt.Id;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.NotNull(loaded);
        Assert.Equal(EventStatus.CANCELLED, loaded.Status);
    }

    [Fact]
    public async Task GivenPublicEvent_WhenSavedAndLoaded_ThenVisibilityIsPublic()
    {
        var evt = MakeDraftEvent();
        evt.MakePublic();
        var savedId = evt.Id;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.NotNull(loaded);
        Assert.Equal(EventVisibility.PUBLIC, loaded.EventVisibility);
    }

    [Fact]
    public async Task GivenEventWithUpdatedTitle_WhenSavedAndLoaded_ThenTitleMatches()
    {
        var evt = MakeDraftEvent();
        var titleResult = EventTitle.Create("Integration Test Event");
        var title = ((Success<EventTitle>)titleResult).Value;
        evt.UpdateTitle(title);
        var savedId = evt.Id;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.NotNull(loaded);
        Assert.Equal("Integration Test Event", loaded.Title.Value);
    }

    [Fact]
    public async Task GivenNonExistentId_WhenGetAsync_ThenReturnsNull()
    {
        await using var ctx = MakeContext();
        var repo = new VeaEventEfcRepository(ctx);
        var fakeId = ((Success<EventId>)EventId.New()).Value;

        var result = await repo.GetAsync(fakeId);

        Assert.Null(result);
    }

    // ── REMOVE ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GivenSavedEvent_WhenRemoved_ThenCannotBeLoaded()
    {
        var evt = MakeDraftEvent();
        var savedId = evt.Id;

        await using (var saveCtx = MakeContext())
        {
            await new VeaEventEfcRepository(saveCtx).AddAsync(evt);
            await new SqliteUnitOfWork(saveCtx).SaveChangesAsync();
        }

        await using (var removeCtx = MakeContext())
        {
            var removeRepo = new VeaEventEfcRepository(removeCtx);
            await removeRepo.RemoveAsync(savedId);
            await new SqliteUnitOfWork(removeCtx).SaveChangesAsync();
        }

        await using var loadCtx = MakeContext();
        var loaded = await new VeaEventEfcRepository(loadCtx).GetAsync(savedId);

        Assert.Null(loaded);
    }
}