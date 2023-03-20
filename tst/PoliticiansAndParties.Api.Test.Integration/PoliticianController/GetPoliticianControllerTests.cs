namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController;

public class GetPoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public GetPoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory) =>
        _client = apiFactory.CreateClient();

    [Fact]
    public async Task Get_ReturnsPolitician_WhenPoliticianExists()
    {
        // Arrange
        var createdPolitician = await Helpers.CreatePoliticianWithParty(_client);

        // Act
        var response = await _client.GetAsync($"api/political-parties/politician/{createdPolitician.Id}");

        // Assert
        var politicianResponse = await response.Content.ReadFromJsonAsync<PoliticianResponse>();
        _ = politicianResponse.Should().BeEquivalentTo(createdPolitician, Helpers.GetDateTimeConfig);
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNoPoliticianExist()
    {
        // Act
        var response = await _client.GetAsync("api/political-parties/politician/" + Guid.NewGuid());

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
