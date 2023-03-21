namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

public class GetPoliticalPartiesControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public GetPoliticalPartiesControllerTests(PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory) =>
        _client = politiciansAndPartiesApiFactory.CreateClient();

    [Fact]
    public async Task GetPoliticalParties_ReturnsEmptyList_WhenNoPoliticalPartyExist()
    {
        // Act
        var response = await _client.GetAsync("api/political-parties");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<PartySideNavResponse>>();
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPoliticalParties_ReturnsPoliticalParties_WhenPartiesExist()
    {
        // Arrange
        var politicalParties = TestData.PartyGenerator.GenerateBetween(5, 10);
        var expectedParties = new List<PartySideNavResponse>();

        foreach (var party in politicalParties)
        {
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", party);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PartySideNavResponse>();

            expectedParties.Add(createdParty!);
        }

        // Act
        var response = await _client.GetAsync("api/political-parties");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<PartySideNavResponse>>();
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = result.Should().BeEquivalentTo(expectedParties);

        // TODO: Should be handle by Spawner for restoring DB to previous state
        // Cleanup
        foreach (var party in expectedParties)
        {
            _ = await _client.DeleteAsync($"api/political-parties/{party.Id}");
        }
    }
}
