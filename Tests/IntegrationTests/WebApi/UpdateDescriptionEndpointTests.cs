using System.Net;
using System.Net.Http.Json;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.WebApi;

public class UpdateDescriptionEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public UpdateDescriptionEndpointTests(WebApiFactory factory)
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
    public async Task GivenExistingEvent_WhenUpdateDescriptionValid_ThenReturns204()
    {
        var id = await CreateEventIdAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{id}/update-description",
            new { Description = "A valid description for the event." });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GivenDescriptionTooLong_WhenUpdateDescription_ThenReturns400()
    {
        var tooLong = new string('x', 251);

        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-description",
            new { Description = tooLong });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenNonExistentEvent_WhenUpdateDescriptionValid_ThenReturns400()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-description",
            new { Description = "Some description." });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenFaultingRepository_WhenUpdateDescription_ThenReturns500()
    {
        await using var brokenFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEventRepository>();
                services.AddScoped<IEventRepository, FaultingEventRepository>();
            }));

        var client = brokenFactory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"/api/events/{Guid.NewGuid()}/update-description",
            new { Description = "Some description." });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}