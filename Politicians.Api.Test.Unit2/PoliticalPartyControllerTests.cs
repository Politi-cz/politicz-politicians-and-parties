using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.Exceptions;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Controllers;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Politicians.Api.Test.Unit
{
    public class PoliticalPartyControllerTests
    {
        private readonly PoliticalPartyController _sut;
        private readonly IPoliticianService _politicianService = Substitute.For<IPoliticianService>();
        private readonly IPoliticalPartyService _politicalPartyService = Substitute.For<IPoliticalPartyService>();

        public PoliticalPartyControllerTests()
        {
            _sut = new PoliticalPartyController(_politicalPartyService, _politicianService);
        }

        [Fact]
        public async Task GetPolitician_ReturnsOkAndObject_WhenPoliticianExists() {
            // Arrange
            var politician = new PoliticianDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                FacebookUrl = "https://facebook.com/testUser",
                TwitterUrl = "https://twitter.com/musk"
            };
            _politicianService.GetAsync(politician.Id).Returns(politician);

            // Act
            var result = (OkObjectResult)await _sut.GetPolitician(politician.Id);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(politician);
        }

        [Fact]
        public async Task GetPolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            _politicianService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = (NotFoundResult)await _sut.GetPolitician(Arg.Any<Guid>());

            // Assert
            // TODO Add constants instead of number
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPoliticalParty_ReturnsOkObject_WhenPartyExists() {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://testingurl.com",
                Name = "KAKAKA",
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto{
                        Id= Guid.NewGuid(),
                        BirthDate= DateTime.Now,
                        FullName = "Karel Jejda",
                        TwitterUrl = "twtr.com"
                    },
                    new PoliticianDto{
                        Id= Guid.NewGuid(),
                        BirthDate= new DateTime(2000,11,30),
                        FullName = "John Bestot",
                        InstagramUrl = "ig.com"
                    }
                }
            };

            _politicalPartyService.GetOneAsync(politicalPartyDto.Id).Returns(politicalPartyDto);

            // Act

            var result = (OkObjectResult)await _sut.GetPoliticalParty(politicalPartyDto.Id);

            // Assert
            result.Value.Should().BeEquivalentTo(politicalPartyDto);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetPoliticalParty_ReturnsNotFound_WhenPartyDesNotExist() {
            // Arrange
            _politicalPartyService.GetOneAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act

            var result = (NotFoundResult)await _sut.GetPoliticalParty(Arg.Any<Guid>());

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsOkObject_WhenPartyExists() {
            // Arrange
            IEnumerable<PoliticalPartySideNavDto> politicalPartiesSideNav = new List<PoliticalPartySideNavDto>() {
                new PoliticalPartySideNavDto{
                    Id = Guid.NewGuid(),
                    Name= "Test",
                    ImageUrl = "img.com"
                },
                new PoliticalPartySideNavDto{
                    Id= Guid.NewGuid(),
                    Name= "Test2",
                    ImageUrl = "randomUIrl.com"
                }
            };

            _politicalPartyService.GetAllAsync().Returns(politicalPartiesSideNav);

            // Act
            var result = (OkObjectResult)await _sut.GetPoliticalParties();

            // Assert
            result.Value.Should().BeEquivalentTo(politicalPartiesSideNav);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            
        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsOkEmptyObject_WhenPartyDoesNotExist()
        {
            // Arrange
            _politicalPartyService.GetAllAsync().Returns(Enumerable.Empty<PoliticalPartySideNavDto>());

            // Act
            var result = (OkObjectResult)await _sut.GetPoliticalParties();

            // Assert
            result.Value.Should().BeEquivalentTo(Enumerable.Empty<PoliticalPartySideNavDto>());
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        //[Fact]
        //public async Task CreatePoliticalParty_ShouldReturnPoliticalParty_WhenPoliticalPartyCreated() { }

        //[Fact]
        //public async Task CreatePoliticalParty_ShouldReturnInternalServerError_WhenPoliticalPartyNotCreated() { }

        //[Fact]
        //public async Task GetPolitician_ShouldReturnNotFound_WhenPoliticianDoesNotExist() { }

        //[Fact]
        //public async Task GetPolitician_ShouldReturnOkObject_WhenPoliticianExist() { }

        //[Fact]
        //public async Task CreatePolitician_ShouldReturnStatusCode500_WhenPoliticianNotCreated() { }

        //[Fact]
        //public async Task CreatePolitician_ShouldReturnPolitician_WhenPoliticianCreated() { }
    }
}
