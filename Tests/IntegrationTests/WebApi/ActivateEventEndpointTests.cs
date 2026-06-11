using System.Net;
using System.Net.Http.Json;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class ActivateEventEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public ActivateEventEndpointTests(WebApiFactory factory)
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

    private async Task SetTitleAsync(string id, string title)
    {
        var resp = await _client.PostAsJsonAsync($"/api/events/{id}/update-title", new { Title = title });
        resp.EnsureSuccessStatusCode();
    }

    private async Task SetTimesAsync(string id)
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var resp = await _client.PostAsJsonAsync($"/api/events/{id}/update-times", new
        {
            StartDate = future.ToString("yyyy-MM-dd"),
            StartTime = "10:00:00",
            EndDate = future.ToString("yyyy-MM-dd"),
            EndTime = "14:00:00"
        });
        resp.EnsureSuccessStatusCode();
    }

    // Success: valid draft event (title + times set) → auto-readied → 204 NoContent
    [Fact]
    public async Task GivenValidDraftEvent_WhenActivateEvent_ThenReturns204()
    {
        var id = await CreateEventIdAsync();
        await SetTitleAsync(id, "VIA Summer Gala");
        await SetTimesAsync(id);

        var response = await _client.PostAsync($"/api/events/{id}/activate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Bad input: non-GUID id → command creation fails → 400
    [Fact]
    public async Task GivenInvalidId_WhenActivateEvent_ThenReturns400()
    {
        var response = await _client.PostAsync("/api/events/not-a-valid-guid/activate", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Business logic failure: default title → ready check fails → 400
    [Fact]
    public async Task GivenDraftEventWithDefaultTitle_WhenActivateEvent_ThenReturns400()
    {
        var id = await CreateEventIdAsync();
        await SetTimesAsync(id);

        var response = await _client.PostAsync($"/api/events/{id}/activate", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Business logic failure: event not found → 400
    [Fact]
    public async Task GivenNonExistentEvent_WhenActivateEvent_ThenReturns400()
    {
        var response = await _client.PostAsync($"/api/events/{Guid.NewGuid()}/activate", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Exception: faulting repository throws → 500 InternalServerError
    [Fact]
    public async Task GivenFaultingRepository_WhenActivateEvent_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsync($"/api/events/{Guid.NewGuid()}/activate", null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}