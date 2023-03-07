using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

public class GetPoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public GetPoliticalPartyControllerTests(
        PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory) =>
        _client = politiciansAndPartiesApiFactory.CreateClient();

    [Fact]
    public async Task GetPoliticalParty_ReturnsPartyWithPoliticians_WhenPartyExists()
    {
        // Arrange
        var generatedParty = DataGenerator.GeneratePoliticalParty();
        var createPartyResponse =
            await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
        var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

        // Act
        var response = await _client.GetAsync("api/political-parties/" + createdParty!.Id);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PoliticalPartyDto>();
        result.Should().BeEquivalentTo(createdParty,
            options => options
                .Using<DateTime>(ctx =>
                    ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPoliticalParty_ReturnsNotFound_WHenPoliticalPartyDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("api/political-parties/" + Guid.NewGuid());
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
