using System.Net;
using System.Net.Http.Json;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class UpdateTimesEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public UpdateTimesEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.InitDbAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<string> CreateEventIdAsync()
    {
        var resp = await _client.PostAsync("/api/events/create-event", null);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadAsStringAsync()).Trim('"');
    }

    private static object ValidFutureTimes()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        return new
        {
            StartDate = future.ToString("yyyy-MM-dd"),
            StartTime = "10:00:00",
            EndDate = future.ToString("yyyy-MM-dd"),
            EndTime = "12:00:00"
        };
    }

    [Fact]
    public async Task GivenExistingEvent_WhenUpdateTimesValid_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{id}/update-times",
            ValidFutureTimes());

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GivenStartInPast_WhenUpdateTimes_ThenReturns400()
    {
        var id = await CreateEventIdAsync();
        var past = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{id}/update-times",
            new
            {
                StartDate = past.ToString("yyyy-MM-dd"),
                StartTime = "10:00:00",
                EndDate = past.ToString("yyyy-MM-dd"),
                EndTime = "12:00:00"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenNonExistentEvent_WhenUpdateTimesValid_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-times",
            ValidFutureTimes());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenFaultingRepository_WhenUpdateTimes_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-times",
            ValidFutureTimes());

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}