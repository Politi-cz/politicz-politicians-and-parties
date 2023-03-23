namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

[Collection("Shared test collection")]
public class GetPoliticalPartyControllerTests
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public GetPoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task GetPoliticalParty_ReturnsPartyWithPoliticians_WhenPartyExists()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);

        // Act
        var response = await _client.GetAsync($"api/political-parties/{createdParty.Id}");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PoliticalPartyResponse>();
        _ = result.Should().BeEquivalentTo(createdParty, Helpers.GetDateTimeConfig);
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPoliticalParty_ReturnsNotFound_WhenPoliticalPartyDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"api/political-parties/{Guid.NewGuid()}");

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
