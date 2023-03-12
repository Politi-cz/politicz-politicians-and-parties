using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class DeletePoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        readonly HttpClient _client;

        public DeletePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public async Task DeleteAsync_DeletesParty_WhenExists()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            // Act
            var getPartyBeforeDelete = await _client.GetAsync($"api/political-parties/{createdParty!.Id}");
            var deletePartyResponse = await _client.DeleteAsync($"api/political-parties/{createdParty!.Id}");
            var getPartyAfterDelete = await _client.GetAsync($"api/political-parties/{createdParty!.Id}");

            // Assert
            getPartyBeforeDelete.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletePartyResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            getPartyAfterDelete.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenPartyDoesNotExist()
        {
            // Arrange
            var nonExistingPartyId = Guid.NewGuid();

            // Act
            var deletePartyResponse = await _client.DeleteAsync($"api/political-parties/{nonExistingPartyId}");

            // Assert
            deletePartyResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
