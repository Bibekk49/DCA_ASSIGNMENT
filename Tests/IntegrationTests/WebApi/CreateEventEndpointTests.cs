using System.Net;

namespace IntegrationTests.WebApi;

public class CreateEventEndpointTests : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public CreateEventEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.InitDbAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GivenRequest_WhenCreateEvent_ThenReturns200WithValidGuid()
    {
        var response = await _client.PostAsync("/api/events/create-event", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadAsStringAsync()).Trim('"');
        Assert.True(Guid.TryParse(body, out _));
    }
}