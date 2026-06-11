using System.Net;
using System.Net.Http.Json;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class UpdateTitleEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public UpdateTitleEndpointTests(WebApiFactory factory)
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

    // Success: existing event + valid title → 204 NoContent
    [Fact]
    public async Task GivenExistingEvent_WhenUpdateTitleValid_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{id}/update-title",
            new { Title = "A Valid Event Title" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Bad input: title too short → command creation fails → 400
    [Fact]
    public async Task GivenTitleTooShort_WhenUpdateTitle_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-title",
            new { Title = "AB" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Business logic failure: event doesn't exist → handler returns NotFound → 400
    [Fact]
    public async Task GivenNonExistentEvent_WhenUpdateTitleValid_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-title",
            new { Title = "Valid Title Here" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Exception scenario: faulting repository throws → ASP.NET Core returns 500
    [Fact]
    public async Task GivenFaultingRepository_WhenUpdateTitle_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-title",
            new { Title = "Valid Title Here" });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}