namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController;

[Collection("Shared test collection")]
public class UpdatePoliticianControllerTests
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public UpdatePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPolitician_WhenDataValid()
    {
        // Arrange
        var createdPolitician = await Helpers.CreatePoliticianWithParty(_client);
        var updatedPolitician = TestData.PoliticianGenerator.Generate();

        // Act
        var updatePoliticianResponse = await _client.PutAsJsonAsync(
            $"api/political-parties/politician/{createdPolitician.Id}",
            updatedPolitician);
        var getPoliticianResponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician.Id}");

        // Assert
        _ = updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await updatePoliticianResponse.Content.ReadFromJsonAsync<PoliticianResponse>();
        _ = result.Should().BeEquivalentTo(updatedPolitician, Helpers.GetDateTimeConfig);

        _ = getPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedPolitician = await getPoliticianResponse.Content.ReadFromJsonAsync<PoliticianResponse>();
        _ = returnedPolitician.Should().BeEquivalentTo(result, Helpers.GetDateTimeConfig);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsErrorDetails_WhenDataInvalid()
    {
        // Arrange
        var createdPolitician = await Helpers.CreatePoliticianWithParty(_client);
        var updatedPolitician = TestData.PoliticianGenerator.Generate() with { InstagramUrl = "nonValidUrl" };

        var expectedError = new ErrorDetail("Validation error");

        // Act
        var updatePoliticianResponse =
            await _client.PutAsJsonAsync(
                $"api/political-parties/politician/{createdPolitician.Id}",
                updatedPolitician);

        // Assert
        _ = updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await updatePoliticianResponse.Content.ReadFromJsonAsync<ErrorDetail>();
        _ = result!.Message.Should().BeEquivalentTo(expectedError.Message);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenPoliticianNotFound()
    {
        // Arrange
        var updatedPolitician = TestData.PoliticianGenerator.Generate();
        var nonExistingId = Guid.NewGuid();

        // Act
        var updatePoliticianResponse =
            await _client.PutAsJsonAsync(
                $"api/political-parties/politician/{nonExistingId}",
                updatedPolitician);

        // Assert
        _ = updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
