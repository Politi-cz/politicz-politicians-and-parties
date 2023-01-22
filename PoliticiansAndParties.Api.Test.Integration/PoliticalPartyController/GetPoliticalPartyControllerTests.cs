using Azure;
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

    public class GetPoliticalPartyControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        private readonly HttpClient _client;

        public GetPoliticalPartyControllerTests(PoliticiansAndPartiesApiFactory politiciansAndPartiesApiFactory)
        {
            _client = politiciansAndPartiesApiFactory.CreateClient();
        }

        // TODO data will be seeded through migration, remove when endpoint for creating political parties/politicians is done
        [Fact]
        public async Task GetPoliticalParty_ReturnsPartyWithPoliticians_WhenPartyAndPoliciansExist() {
              // Arrange
            var politicalParty = new PoliticalPartyDto
            {
                Id = Guid.Parse("24ed715a-31cb-4fca-9726-533072b0c79d"),
                Name = "ANO",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/92/ANO_Logo.svg/1200px-ANO_Logo.svg.png",
                Politicians = new List<PoliticianDto>
                {
                    new PoliticianDto{
                        Id = Guid.Parse("4756c20d-826f-4c7c-89d0-3252e130546d"),
                        FullName = "Andrej",
                        BirthDate = new DateTime(1962, 05, 05),
                    },
                    new PoliticianDto
                    {
                        Id = Guid.Parse("65e86328-5f07-45b6-b5fe-4e6be3d98d2f"),
                        FullName = "Karel",
                        BirthDate = new DateTime(1977, 04, 05),
                    }
                }
            };

            // Act

            var response = await _client.GetAsync("api/political-parties/" + politicalParty.Id.ToString());

            // Assert
            var result = await response.Content.ReadFromJsonAsync<PoliticalPartyDto>();
            result.Should().BeEquivalentTo(politicalParty);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetPoliticalParty_ReturnsPartyWithNoPoliticians_WhenPartyExistWithoutPoliticians()
        {
            // Arrange
            var politicalParty = new PoliticalPartyDto
            {
                Id = new Guid("bfc3975f-e3bb-4511-bd24-88c9e0f457b1"),
                Name = "ODS",
                ImageUrl = "https://www.ods.cz/img/logo/ods-logo-prechod.jpg"
            };

            // Act

            var response = await _client.GetAsync("api/political-parties/" + politicalParty.Id.ToString());

            // Assert
            var result = await response.Content.ReadFromJsonAsync<PoliticalPartyDto>();
            result.Should().BeEquivalentTo(politicalParty);
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
