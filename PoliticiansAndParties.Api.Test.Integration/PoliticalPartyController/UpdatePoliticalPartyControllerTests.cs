using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class UpdatePoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        readonly HttpClient _client;

        public UpdatePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task UpdatePoliticalParty_ReturnsErrorDetails_WhenInvalidData()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var updatedParty = new UpdatePoliticalPartyDto
            {
                Name = "",// Invalid, empty name
                ImageUrl = "https://newTestUrl.com",
                Tags = new HashSet<string>() // Empty set, invalid
            };

            var expectedResponse = new ErrorDetail("Validation error")
            {
                Errors = new Dictionary<string, string[]> {
                    { "Name", new[] { "'Name' must not be empty." }},
                    { "Tags", new[] { "'Tags' must not be empty." }}
                }
            };

            // Act
            var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty!.Id}", updatedParty);

            // Assert
            updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorDetails = await updatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();
            errorDetails.Should().BeEquivalentTo(expectedResponse);

        }

        [Fact]
        public async Task UpdatePoliticalParty_ReturnsErrorDetails_WhenUpdatedPartyNameExists()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var expectedError = new ErrorDetail("Validation error")
            {
                Errors = new Dictionary<string, string[]> { { "Name", new[] { $"Political party with name {generatedParty.Name} already exists" } } }
            };

            var partyData = DataGenerator.GeneratePoliticalParty();
            var updatedParty = new UpdatePoliticalPartyDto
            {
                Name = createdParty!.Name,
                ImageUrl = partyData.ImageUrl,
                Tags = partyData.Tags,
            };

            // Act
            var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty.Id}", updatedParty);

            // Assert
            updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorDetails = await updatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();
            errorDetails.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        public async Task UpdatePoliticalParty_ReturnsNotFound_WhenUpdatedPartyDoesNotExist()
        {
            // Arrange
            var nonExistingParty = new UpdatePoliticalPartyDto
            {
                Name = "test",
                ImageUrl = "https://test.com",
                Tags = new HashSet<string> { "Name" }
            };
            var nonExistingGuid = Guid.NewGuid();

            // Act
            var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{nonExistingGuid}", nonExistingParty);

            // Assert
            updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePoliticalParty_UpdatesPoliticalParty_WhenDataValid()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var updatedTags = generatedParty.Tags;
            updatedTags.Add("New updated tag");
            var updatedParty = new UpdatePoliticalPartyDto
            {
                Id = createdParty!.Id,
                Name = "Updated name",
                ImageUrl = "https://updastedurl.com",
                Tags = updatedTags
            };

            // Act
            var updatePartyResponse = await _client.PutAsJsonAsync($"api/political-parties/{createdParty!.Id}", updatedParty);
            var getPartyResponse = await _client.GetAsync($"api/political-parties/{createdParty!.Id}");

            // Assert
            updatePartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await updatePartyResponse.Content.ReadFromJsonAsync<UpdatePoliticalPartyDto>();
            result.Should().BeEquivalentTo(updatedParty);

            getPartyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getPartyAfterUpdate = await getPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();
            getPartyAfterUpdate.Should().BeEquivalentTo(result);

        }
    }
}
