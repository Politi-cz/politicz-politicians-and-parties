using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController
{
    public class GetPoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        private readonly HttpClient _client;

        public GetPoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsPolitician_WhenPoliticianExists()
        {
            // Arrange
            var party = DataGenerator.GeneratePoliticalParty(1);
            var politician = DataGenerator.GeneratePolitician();

            var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", party);
            var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

            var createPoliticianResponse = await _client.PostAsJsonAsync($"api/political-parties/{createdParty!.Id}/politician", politician);
            var createdPolitician = await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();


            // Act
            var response = await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");


            // Assert
            var politicianResponse = await response.Content.ReadFromJsonAsync<PoliticianDto>();
            politicianResponse.Should().BeEquivalentTo(createdPolitician, options => options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100))).WhenTypeIs<DateTime>());
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoPoliticianExist()
        {
            // Act
            var response = await _client.GetAsync("api/political-parties/politician/" + Guid.NewGuid());


            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
