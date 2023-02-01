using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using politicz_politicians_and_parties.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliticiansAndParties.Api.Test.Unit
{
    public class PoliticalPartyServiceTests
    {
        private readonly PoliticalPartyService _sut;
        private readonly IPoliticalPartyRepository _politicianPartyRepository = Substitute.For<IPoliticalPartyRepository>();

        public PoliticalPartyServiceTests()
        {
            _sut = new PoliticalPartyService(_politicianPartyRepository, new PoliticalPartyDtoValidator());
        }

        [Fact]
        public async Task GetOneAsync_ReturnsPoliticalPartyDto_WhenPartyExists()
        {
            // Arrange
            var politicalParty = new PoliticalParty
            {
                Id = 1,
                FrontEndId = Guid.NewGuid(),
                ImageUrl = "img.com",
                Name = "Spolu",
                Politicians = new List<Politician> {
                    new Politician{
                        Id = 1,
                        FrontEndId= Guid.NewGuid(),
                        BirthDate= DateTime.Now,
                        InstagramUrl = "ig.com/pepaKarel",
                        FullName = "Pepa Karel",
                        PoliticalPartyId = 1
                    }
                }
            };

            var expectedResult = politicalParty.ToPoliticalPartyDto();

            _politicianPartyRepository.GetAsync(politicalParty.FrontEndId).Returns(politicalParty);

            // Act
            var result = await _sut.GetOneAsync(politicalParty.FrontEndId);
            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetOneAsync_ReturnsNull_WhenPartyDoesNotExist()
        {
            // Arrange
            _politicianPartyRepository.GetAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = await _sut.GetOneAsync(Arg.Any<Guid>());
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEnumerableOfSideNavDto_WhenPartiesExist()
        {
            // Arrange
            var politicalParties = new List<PoliticalParty>(){ 
                new PoliticalParty{
                    Id = 1,
                    FrontEndId = Guid.NewGuid(),
                    ImageUrl = "img.com",
                    Name = "Spolu",
                    Politicians = new List<Politician> {
                        new Politician{
                            Id = 1,
                            FrontEndId= Guid.NewGuid(),
                            BirthDate= DateTime.Now,
                            InstagramUrl = "ig.com/pepaKarel",
                            FullName = "Pepa Karel",
                            PoliticalPartyId = 1
                        }
                    }
                },
                new PoliticalParty{
                    Id = 2,
                    FrontEndId = Guid.NewGuid(),
                    ImageUrl = "img.com/2",
                    Name = "Teeest",
                }
            };

            IEnumerable<PoliticalPartySideNavDto> expectedResult = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

            _politicianPartyRepository.GetAllAsync().Returns(politicalParties);

            // Act
            var result = await _sut.GetAllAsync();
            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetPoliticalPartiesAsync_ReturnsEmptyIEnumerable_WhenPartyDoesNotExist()
        {
            // Arrange
            _politicianPartyRepository.GetAllAsync().Returns(Enumerable.Empty<PoliticalParty>());
            // Act

            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        //[Fact]
        //public async Task CreateAsync_ShouldThrowValidationError_WhenInvalidPoliticianDto() { 
            
        //}

        //[Fact]
        //public async Task CreateAsync_ShouldThrowValidationException_WhenPoliticalPartyWithSameNameExists() { 
        
        //}

        //[Fact]
        //public async Task CreateAsync_ShouldReturnTrue_WhenPoliticalPartyCreated() { 

        //}

        //[Fact]
        //public async Task CreateAsync_ShouldReturnFalse_WhenPoliticalPartyNotCreated() { 
        //}



    }
}
