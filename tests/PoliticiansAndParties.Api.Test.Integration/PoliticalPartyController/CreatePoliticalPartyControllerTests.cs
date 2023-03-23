namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

[Collection("Shared test collection")]
public class CreatePoliticalPartyControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public CreatePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task CreatePoliticalParty_CreatesParty_WhenDataAreValid()
    {
        // Arrange
        var generatedParty = TestData.PartyGenerator.Generate();

        // Act
        var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
        var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyResponse>();
        var getPartyResponse = await _client.GetAsync($"api/political-parties/{createdParty!.Id}");
        var returnedParty = await getPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyResponse>();

        // Assert
        _ = createPartyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        _ = getPartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        _ = createdParty.Should().BeEquivalentTo(returnedParty, Helpers.GetDateTimeConfig);
    }

    [Fact]
    public async Task CreatePoliticalParty_ReturnsErrorDetails_WhenDataAreInvalid()
    {
        // Arrange
        var generatedParty = TestData.GetInvalidParty;
        var expectedError = new ErrorDetail("Validation error");

        // Act
        var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
        var errorDetails = await createPartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();

        // Assert
        _ = createPartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = errorDetails!.Message.Should().BeEquivalentTo(expectedError.Message);
    }

    [Fact]
    public async Task CreatePoliticalParty_ReturnsErrorDetails_WhenPoliticalPartyAlreadyExists()
    {
        // Arrange
        var generatedParty = TestData.PartyGenerator.Generate();
        _ = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);

        var expectedError = new ErrorDetail($"Political party with name {generatedParty.Name} already exists");

        // Act
        var createDuplicatePartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
        var errorDetails = await createDuplicatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();

        // Assert
        _ = createDuplicatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = errorDetails.Should().BeEquivalentTo(expectedError);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
