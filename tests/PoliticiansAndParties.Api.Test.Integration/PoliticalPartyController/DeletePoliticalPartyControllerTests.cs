namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

[Collection("Shared test collection")]
public class DeletePoliticalPartyControllerTests
{
    private readonly HttpClient _client;

    private readonly Func<Task> _resetDatabase;

    public DeletePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

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

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
