using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class CreatePoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        readonly HttpClient _client;

        public CreatePoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public async Task CreatePoliticalParty_CreatesParty_WhenDataAreValid()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();

            // Act
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();
            var getPartyResponse = await _client.GetAsync($"api/political-parties/{createdParty!.Id}");
            var returnedParty = await getPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            // Assert
            createPartyResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            getPartyResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            createdParty.Should().BeEquivalentTo(returnedParty, options => options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100))).WhenTypeIs<DateTime>());
        }

        [Fact]
        public async Task CreatePoliticalParty_ReturnsErrorDetails_WhenDataAreInvalid()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var expectedError = new ErrorDetail("Validation error")
            {
                Errors = new Dictionary<string, string[]> {
                    {"Politicians", new []{"'Politicians' must not be empty." } }
                },
            };

            generatedParty.Politicians = new List<PoliticianDto>(); // empty list of politicians is invalid

            // Act
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var errorDetails = await createPartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();

            // Assert
            createPartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            errorDetails.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        public async Task CreatePoliticalParty_ReturnsErrorDetails_WhenPoliticalPartyAlreadyExists()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);

            var expectedError = new ErrorDetail("Validation error")
            {
                Errors = new Dictionary<string, string[]> {
                    {"Name", new []{$"Political party with name {generatedParty.Name} already exists" } }
                }
            };

            // Act
            var createDuplicatePartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var errorDetails = await createDuplicatePartyResponse.Content.ReadFromJsonAsync<ErrorDetail>();

            // Assert
            createDuplicatePartyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            errorDetails.Should().BeEquivalentTo(expectedError);
        }
    }
}
