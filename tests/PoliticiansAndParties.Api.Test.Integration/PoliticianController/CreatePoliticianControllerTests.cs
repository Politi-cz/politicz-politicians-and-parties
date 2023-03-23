namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController;

[Collection("Shared test collection")]
public class CreatePoliticianControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public CreatePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task CreatePolitician_CreatesPolitician_WhenDataAreValid()
    {
        // Arrange
        var createdParty = await Helpers.CreatePoliticalParty(_client);
        var generatedPolitician = TestData.PoliticianGenerator.Generate();

        // Act
        var createPoliticianResponse =
            await _client.PostAsJsonAsync(
                $"api/political-parties/{createdParty.Id}/politician",
                generatedPolitician);
        var createdPolitician = await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianResponse>();
        var getPoliticianResponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");
        var returnedPolitician = await getPoliticianResponse.Content.ReadFromJsonAsync<PoliticianResponse>();

        // Assert
        _ = createPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        _ = getPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        _ = returnedPolitician.Should().BeEquivalentTo(createdPolitician, Helpers.GetDateTimeConfig);
    }

    [Fact]
    public async Task CreatePolitician_ReturnsErrorDetails_WhenDataAreInvalid()
    {
        // Arrange
        var generatedPolitician = TestData.PoliticianGenerator.Generate() with { FacebookUrl = "invalidUrl" };
        var expectedError = new ErrorDetail(
            "Validation error",
            new Dictionary<string, string[]>
            {
                { "FacebookUrl", new[] { "Must be a valid url." } },
            });

        // Act
        var createPoliticianResponse =
            await _client.PostAsJsonAsync(
                $"api/political-parties/{Guid.NewGuid()}/politician",
                generatedPolitician);
        var errorDetails = await createPoliticianResponse.Content.ReadFromJsonAsync<ErrorDetail>();

        // Assert
        _ = createPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = errorDetails.Should().BeEquivalentTo(expectedError);
    }

    [Fact]
    public async Task CreatePolitician_ReturnsErrorDetails_WhenAssignedPoliticalPartyDoesNotExist()
    {
        // Arrange
        var generatedPolitician = TestData.PoliticianGenerator.Generate();
        var nonExistingPartyId = Guid.NewGuid();
        var expectedError = new ErrorDetail($"Political party with id {nonExistingPartyId} does not exist");

        // Act
        var createPoliticianResponse =
            await _client.PostAsJsonAsync(
                $"api/political-parties/{nonExistingPartyId}/politician",
                generatedPolitician);
        var errorDetails = await createPoliticianResponse.Content.ReadFromJsonAsync<ErrorDetail>();

        // Assert
        _ = createPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = errorDetails.Should().BeEquivalentTo(expectedError);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
