using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController
{
    public class DeletePoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        HttpClient _client;

        public DeletePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public async Task DeleteAsync_DeletesPolitician_WhenPoliticianExists()
        {
            // Arrange
            var parentParty = DataGenerator.GeneratePoliticalParty(1);
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", parentParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var generatedPolitician = DataGenerator.GeneratePolitician();
            var createPoliticianResponse = await _client.PostAsJsonAsync($"api/political-parties/{createdParty!.Id}/politician", generatedPolitician);
            var createdPolitician = await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();

            // Act
            var getPoliticianReponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");
            var deletePoliticianResponse = await _client.DeleteAsync($"api/political-parties/politician/{createdPolitician!.Id}");
            var getPoliticianAfterDeleteReponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");

            // Assert
            getPoliticianReponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletePoliticianResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            getPoliticianAfterDeleteReponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var nonExistingPoliticianId = Guid.NewGuid();

            // Act
            var deletePoliticianResponse = await _client.DeleteAsync($"api/political-parties/politician/{nonExistingPoliticianId}");

            // Assert
            deletePoliticianResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
