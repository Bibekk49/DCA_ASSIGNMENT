using System.Net;
using System.Net.Http.Json;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class SetMaxGuestsEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public SetMaxGuestsEndpointTests(WebApiFactory factory)
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

    // Success: existing DRAFT event + valid max guests → 204 NoContent
    [Fact]
    public async Task GivenExistingDraftEvent_WhenSetMaxGuestsValid_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{id}/set-max-guests",
            new { MaxGuests = 25 });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Bad input: max guests = 0 → command creation fails → 400
    [Fact]
    public async Task GivenMaxGuestsZero_WhenSetMaxGuests_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/set-max-guests",
            new { MaxGuests = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Business logic failure: event not found → handler returns NotFound → 400
    [Fact]
    public async Task GivenNonExistentEvent_WhenSetMaxGuestsValid_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/set-max-guests",
            new { MaxGuests = 10 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Exception: faulting repository throws → 500 InternalServerError
    [Fact]
    public async Task GivenFaultingRepository_WhenSetMaxGuests_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/set-max-guests",
            new { MaxGuests = 10 });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}