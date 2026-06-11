using System.Net;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class MakePrivateEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public MakePrivateEndpointTests(WebApiFactory factory)
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

    [Fact]
    public async Task GivenExistingDraftEvent_WhenMakePrivate_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsync($"/api/events/{id}/make-private", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GivenInvalidId_WhenMakePrivate_ThenReturns400()
    {
        var response = await _client.PostAsync("/api/events/not-a-valid-guid/make-private", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenNonExistentEvent_WhenMakePrivate_ThenReturns400()
    {
        var response = await _client.PostAsync($"/api/events/{Guid.NewGuid()}/make-private", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenFaultingRepository_WhenMakePrivate_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsync($"/api/events/{Guid.NewGuid()}/make-private", null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}