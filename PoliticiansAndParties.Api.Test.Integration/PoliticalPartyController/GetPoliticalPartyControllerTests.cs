using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{

    public class GetPoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        private readonly HttpClient _client;

        public GetPoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory)
        {
            _client = politiciansAndPartiesApiFactory.CreateClient();
        }

        // TODO data will be seeded through migration, remove when endpoint for creating political parties/politicians is done
        [Fact]
        public async Task GetPoliticalParty_ReturnsPartyWithPoliticians_WhenPartyExists()
        {
            // Arrange
            var generatedParty = DataGenerator.GeneratePoliticalParty();
            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", generatedParty);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            // Act
            var response = await _client.GetAsync("api/political-parties/" + createdParty!.Id.ToString());

            // Assert
            var result = await response.Content.ReadFromJsonAsync<PoliticalPartyDto>();
            result.Should().BeEquivalentTo(createdParty, options => options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100))).WhenTypeIs<DateTime>());
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetPoliticalParty_ReturnsNotFound_WHenPoliticalPartyDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("api/political-parties/" + Guid.NewGuid().ToString());
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }
    }
}
