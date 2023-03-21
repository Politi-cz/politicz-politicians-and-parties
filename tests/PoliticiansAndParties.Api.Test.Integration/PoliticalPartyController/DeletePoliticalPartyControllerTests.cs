namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

public class DeletePoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public DeletePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory) =>
        _client = apiFactory.CreateClient();

    [Fact]
    public async Task DeleteAsync_DeletesParty_WhenExists()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);

        // Act
        var getPartyBeforeDelete = await _client.GetAsync($"api/political-parties/{createdParty.Id}");
        var deletePartyResponse = await _client.DeleteAsync($"api/political-parties/{createdParty.Id}");
        var getPartyAfterDelete = await _client.GetAsync($"api/political-parties/{createdParty.Id}");

        // Assert
        _ = getPartyBeforeDelete.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = deletePartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = getPartyAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        // Arrange
        var nonExistingPartyId = Guid.NewGuid();

        // Act
        var deletePartyResponse = await _client.DeleteAsync($"api/political-parties/{nonExistingPartyId}");

        // Assert
        _ = deletePartyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
