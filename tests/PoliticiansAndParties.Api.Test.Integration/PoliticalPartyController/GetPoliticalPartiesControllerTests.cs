namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

[Collection("Shared test collection")]
public class GetPoliticalPartiesControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public GetPoliticalPartiesControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabase;
    }

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
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();
}
