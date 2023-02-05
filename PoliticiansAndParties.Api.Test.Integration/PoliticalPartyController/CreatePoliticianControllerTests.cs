using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class CreatePoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        readonly HttpClient _client;

        public CreatePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public async Task CreatePolitician_CreatesPolitician_WhenDataAreValid()
        {
            // Arrange
            var parentParty = DataGenerator.GeneratePoliticalParty(1);
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", parentParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var generatedPolitician = DataGenerator.GeneratePolitician();

            // Act
            var createPoliticianResponse = await _client.PostAsJsonAsync($"api/political-parties/{createdParty!.Id}/politician", generatedPolitician);
            var createdPolitician = await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();
            var getPoliticianResponse = await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");
            var returnedPolitician = await getPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();

            // Assert
            createPoliticianResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            getPoliticianResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            returnedPolitician.Should().BeEquivalentTo(createdPolitician, options => options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100))).WhenTypeIs<DateTime>());
        }

        [Fact]
        public async Task CreatePolitician_ReturnsErrorDetails_WhenDataAreInvalid()
        {
            // Arrange
            var generatedPolitician = DataGenerator.GeneratePolitician();
            generatedPolitician.FacebookUrl = "invalidUrl";

            var expectedError = new ErrorDetails
            {
                Message = "Validation error",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string> { { "FacebookUrl", "Must be valid url" } }
            };

            // Act
            var createPoliticianResponse = await _client.PostAsJsonAsync($"api/political-parties/{Guid.NewGuid()}/politician", generatedPolitician);
            var errorDetails = await createPoliticianResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // Assert
            createPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            errorDetails.Should().BeEquivalentTo(expectedError);

        }

        [Fact]
        public async Task CreatePolitician_ReturnsErrorDetails_WhenAssignedPoliticalPartyDoesNotExist()
        {
            // Arrange
            var generatedPolitician = DataGenerator.GeneratePolitician();
            var nonExistingPartyId = Guid.NewGuid();
            var expectedError = new ErrorDetails
            {
                Message = "Validation error",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string> { { "Id", $"Political party with id {nonExistingPartyId} does not exist" } }
            };

            // Act
            var createPoliticianResponse = await _client.PostAsJsonAsync($"api/political-parties/{nonExistingPartyId}/politician", generatedPolitician);
            var errorDetails = await createPoliticianResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // Assert
            createPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            errorDetails.Should().BeEquivalentTo(expectedError);
        }
    }
}
