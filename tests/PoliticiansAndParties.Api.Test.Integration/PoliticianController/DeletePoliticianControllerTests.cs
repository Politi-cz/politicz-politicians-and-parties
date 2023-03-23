namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController;

[Collection("Shared test collection")]
public class DeletePoliticianControllerTests
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public DeletePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task DeleteAsync_DeletesPolitician_WhenPoliticianExists()
    {
        // Arrange
        var createdPolitician = await Helpers.CreatePoliticianWithParty(_client);

        // Act
        var getPoliticianResponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician.Id}");
        var deletePoliticianResponse = await _client.DeleteAsync($"api/political-parties/politician/{createdPolitician.Id}");
        var getPoliticianAfterDeleteResponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician.Id}");

        // Assert
        _ = getPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = deletePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = getPoliticianAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenPoliticianDoesNotExist()
    {
        // Arrange
        var nonExistingPoliticianId = Guid.NewGuid();

        // Act
        var deletePoliticianResponse = await _client.DeleteAsync($"api/political-parties/politician/{nonExistingPoliticianId}");

        // Assert
        _ = deletePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
