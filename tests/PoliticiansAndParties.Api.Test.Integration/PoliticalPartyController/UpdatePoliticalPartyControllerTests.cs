namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

[Collection("Shared test collection")]
public class UpdatePoliticalPartyControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public UpdatePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task UpdatePoliticalParty_ReturnsErrorDetails_WhenInvalidData()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);
        var updatedParty = new PoliticalPartyRequest(
            string.Empty,
            "https://newTestUrl.com",
            new HashSet<string>(),
            new List<PoliticianRequest>());

        var expectedResponse = new ErrorDetail("Validation error");

        // Act
        var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty.Id}", updatedParty);

        // Assert
        _ = updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorDetails = await updatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();
        _ = errorDetails!.Message.Should().BeEquivalentTo(expectedResponse.Message);
    }

    [Fact]
    public async Task UpdatePoliticalParty_ReturnsErrorDetails_WhenUpdatedPartyNameExists()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);
        var createdParty2 = await Helpers.CreatePoliticalParty(_client);

        var expectedError = new ErrorDetail($"Political party with name {createdParty.Name} already exists");

        var updatedParty = TestData.PartyGenerator.Generate() with { Name = createdParty.Name };

        // Act
        var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty2.Id}", updatedParty);

        // Assert
        _ = updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorDetails = await updatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();
        _ = errorDetails.Should().BeEquivalentTo(expectedError);
    }

    [Fact]
    public async Task UpdatePoliticalParty_ReturnsNotFound_WhenUpdatedPartyDoesNotExist()
    {
        // Arrange
        var nonExistingParty = TestData.PartyGenerator.Generate();
        var nonExistingGuid = Guid.NewGuid();

        // Act
        var updatePartyResponse = await _client.PutAsJsonAsync(
                $"api/political-parties/{nonExistingGuid}",
                nonExistingParty);

        // Assert
        _ = updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePoliticalParty_UpdatesPoliticalParty_WhenDataValid()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);
        var updatedParty = TestData.PartyGenerator.Generate() with { Name = createdParty.Name, Politicians = new() };

        // Act
        var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty.Id}", updatedParty);
        var getPartyResponse = await _client.GetAsync($"api/political-parties/{createdParty.Id}");

        // Assert
        _ = updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await updatePartyResponse.Content.ReadFromJsonAsync<PoliticalPartyResponse>();
        _ = result.Should().BeEquivalentTo(updatedParty);

        _ = getPartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getPartyAfterUpdate = await getPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyResponse>();
        getPartyAfterUpdate = getPartyAfterUpdate! with { Politicians = Enumerable.Empty<PoliticianResponse>() };
        _ = getPartyAfterUpdate.Should().BeEquivalentTo(result);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
