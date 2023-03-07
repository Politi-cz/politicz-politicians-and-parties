using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController;

public class GetPoliticalPartiesControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public GetPoliticalPartiesControllerTests(
        PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory) =>
        _client = politiciansAndPartiesApiFactory.CreateClient();

    [Fact]
    public async Task GetPoliticalParties_ReturnsEmptyList_WhenNoPoliticalPartyExist()
    {
        // Act
        var response = await _client.GetAsync("api/political-parties");

        // Assert
        var result =
            await response.Content.ReadFromJsonAsync<IEnumerable<PoliticalPartySideNavDto>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPoliticalParties_ReturnsPoliticalParties_WhenPartiesExist()
    {
        // Arrange
        var politicalParties = DataGenerator.GeneratePoliticalParties();
        var expectedParties = new List<PoliticalPartySideNavDto>();

        foreach (var party in politicalParties)
        {
            var createPartyResponse =
                await _client.PostAsJsonAsync("api/political-parties/create", party);

            var createdParty =
                await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            expectedParties.Add(createdParty!.ToPoliticalParty().ToPoliticalPartySideNavDto());
        }

        // Act
        var response = await _client.GetAsync("api/political-parties");

        // Assert
        var result =
            await response.Content.ReadFromJsonAsync<IEnumerable<PoliticalPartySideNavDto>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEquivalentTo(expectedParties);

        // TODO: Should be handle by Spawner for restoring DB to previous state
        // Cleanup
        foreach (var party in expectedParties)
            await _client.DeleteAsync($"api/political-parties/{party.Id}");
    }
}
