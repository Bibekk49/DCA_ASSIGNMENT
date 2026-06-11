using System.Net;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class MakePublicEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public MakePublicEndpointTests(WebApiFactory factory)
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

    // Success: existing DRAFT event → make public → 204 NoContent
    [Fact]
    public async Task GivenExistingDraftEvent_WhenMakePublic_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsync($"/api/events/{id}/make-public", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Bad input: non-GUID id → command creation fails (EventId parse error) → 400
    [Fact]
    public async Task GivenInvalidId_WhenMakePublic_ThenReturns400()
    {
        var response = await _client.PostAsync("/api/events/not-a-valid-guid/make-public", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Business logic failure: event not found in DB → handler returns NotFound → 400
    [Fact]
    public async Task GivenNonExistentEvent_WhenMakePublic_ThenReturns400()
    {
        var response = await _client.PostAsync($"/api/events/{Guid.NewGuid()}/make-public", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Exception: faulting repository throws → 500 InternalServerError
    [Fact]
    public async Task GivenFaultingRepository_WhenMakePublic_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsync($"/api/events/{Guid.NewGuid()}/make-public", null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}