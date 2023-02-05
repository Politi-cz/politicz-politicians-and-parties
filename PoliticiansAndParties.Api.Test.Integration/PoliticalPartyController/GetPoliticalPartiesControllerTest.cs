using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class GetPoliticalPartiesControllerTest : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        readonly HttpClient _client;

        public GetPoliticalPartiesControllerTest(PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory)
        {
            _client = politiciansAndPartiesApiFactory.CreateClient();
        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsEmptyList_WhenNoPoliticalPartyExist()
        {
            // Act
            var response = await _client.GetAsync("api/political-parties");

            // Assert
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<PoliticalPartySideNavDto>>();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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
                var createPartyResponse = await _client.PostAsJsonAsync("api/political-parties/create", party);
                // createPartyResponse.StatusCode.Should().Be(HttpStatusCode.Created, createPartyResponse.Content.ToString());

                var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

                expectedParties.Add(createdParty!.ToPoliticalParty().ToPoliticalPartySideNavDto());
            }

            // Act
            var response = await _client.GetAsync("api/political-parties");

            // Assert
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<PoliticalPartySideNavDto>>();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedParties);

            // TODO after the test parties should be deleted because test above needs clear database. Now it works because of the test executin order
        }


    }
}
