namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

public class GetPoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public GetPoliticalPartyControllerTests(
        PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory) =>
        _client = politiciansAndPartiesApiFactory.CreateClient();

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
}
