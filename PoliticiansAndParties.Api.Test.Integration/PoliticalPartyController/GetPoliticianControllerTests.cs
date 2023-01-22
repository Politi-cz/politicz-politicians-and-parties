using FluentAssertions;
using politicz_politicians_and_parties.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
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
            // TODO SHOULD BE CRREATED BY THE CONTROLLER, NOW USING SEEDED DATA FROM DB
            // N'a5e15559-ebba-426a-8a38-f56e3421903c', N'Tomio', CAST(N'1966-01-01T00:00:00.0000000' AS DateTime2)
            var expectedPolitician = new PoliticianDto
            {
                Id = new Guid("a5e15559-ebba-426a-8a38-f56e3421903c"),
                BirthDate = new DateTime(1966, 1, 1),
                FullName = "Tomio"
            };


            // Act
            var response = await _client.GetAsync("api/political-parties/politician/" + expectedPolitician.Id.ToString());


            // Assert
            var politicianResponse = await response.Content.ReadFromJsonAsync<PoliticianDto>();
            politicianResponse.Should().BeEquivalentTo(expectedPolitician);
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
