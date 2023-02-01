using FluentAssertions;
using FluentValidation;
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

namespace Politicians.Api.Test.Unit
{
    public class PoliticianServiceTests
    {
        private readonly PoliticianService _sut;
        private readonly IPoliticianRepository _politicianRepository = Substitute.For<IPoliticianRepository>();
        private readonly IPoliticalPartyRepository _politicalPartyRepository= Substitute.For<IPoliticalPartyRepository>();

        public PoliticianServiceTests()
        {
            _sut = new PoliticianService(_politicianRepository, _politicalPartyRepository, new PoliticianDtoValidator());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPoliticianDto_WhenPoliticianExists() {
            // Arrange
            var existingPolitician = new Politician {
                Id = 1,
                FrontEndId = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                PoliticalPartyId = 25,
            };
            var expectedPolitician = existingPolitician.ToPoliticianDto();
            _politicianRepository.GetAsync(existingPolitician.FrontEndId).Returns(existingPolitician);

            // Act
            var result = await _sut.GetAsync(existingPolitician.FrontEndId);

            // Assert
            result.Should().BeEquivalentTo(expectedPolitician);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenPoliticianDoesNotExist() { 
            // Arrange
            _politicianRepository.GetAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = await _sut.GetAsync(Arg.Any<Guid>());

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidPoliticianData))]
        public async Task CreateAsync_ShouldThrowValidationError_WhenInvalidPoliticianDto(PoliticianDto politicianDto) {
            // Arrange
            _politicianRepository.CreateOneAsync(politicianDto.ToPolitician()).Returns(true);
            _politicalPartyRepository.GetInternalIdAsync(Arg.Any<Guid>()).Returns(10); 
            // does not really matter

            // Act
            var act = async () => await _sut.CreateAsync(Guid.NewGuid(), politicianDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationError_WhenParentPoliticalPartyDoesNotExist() {
            // Arrange
            var politicianDto = new PoliticianDto()
            {
                FullName = "Test User",
                BirthDate = DateTime.Now,
            };
            var nonExistingPartyId = Guid.NewGuid();

            _politicalPartyRepository.GetInternalIdAsync(nonExistingPartyId).ReturnsNull();
            _politicianRepository.CreateOneAsync(Arg.Any<Politician>()).Returns(false);

            // Act
            var act = async () => await _sut.CreateAsync(nonExistingPartyId, politicianDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Political party with id {nonExistingPartyId} does not exist");
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTrue_WhenPoliticianCreated()
        {
            // Arrange
            var politicianDto = new PoliticianDto()
            {
                FullName = "Test User",
                BirthDate = DateTime.Now,
            };
            var politicalPartyGuid = Guid.NewGuid();

            _politicalPartyRepository.GetInternalIdAsync(politicalPartyGuid).Returns(10);
            _politicianRepository.CreateOneAsync(Arg.Any<Politician>()).Returns(true);

            // Act
            var created = await _sut.CreateAsync(politicalPartyGuid, politicianDto);

            // Assert
            created.Should().Be(true);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFalse_WhenPoliticianWasNotCreated()
        {
            // Arrange
            var politicianDto = new PoliticianDto()
            {
                FullName = "Test User",
                BirthDate = DateTime.Now,
            };
            var politicalPartyGuid = Guid.NewGuid();

            _politicalPartyRepository.GetInternalIdAsync(politicalPartyGuid).Returns(10);
            _politicianRepository.CreateOneAsync(Arg.Any<Politician>()).Returns(false);

            // Act
            var created = await _sut.CreateAsync(politicalPartyGuid, politicianDto);

            // Assert
            created.Should().Be(false);
        }

        public static IEnumerable<object[]> InvalidPoliticianData =>
          new List<object[]>
          {
                    new object[] {new PoliticianDto{ } },
                    new object[] {new PoliticianDto {
                        BirthDate= new DateTime(2000, 12,10),
                        FullName = "", // Validation error
                        FacebookUrl = "https://facebook.com/test",
                        TwitterUrl = "https://twitter.com/test",
                        InstagramUrl = "https://instagram.com/test"
                    } },
                    new object[] { new PoliticianDto{
                        BirthDate= new DateTime(), // Error
                        FullName = "Karel",
                        FacebookUrl = "https://facebook.com/test",
                        TwitterUrl = "https://twitter.com/test",
                        InstagramUrl = "https://instagram.com/test"
                    } },
                    new object[] { new PoliticianDto{
                        BirthDate= new DateTime(2000, 12,10),
                        FullName = "Karel",
                        FacebookUrl = "htt://facebook.com/test", // Invalid url
                    } },
                    new object[] { new PoliticianDto{
                        BirthDate= new DateTime(2000, 12,10),
                        FullName = "Karel",
                        InstagramUrl= "fdsafasd", // Invalid url
                    } },
                    new object[] { new PoliticianDto{
                        BirthDate= new DateTime(2000, 12,10),
                        FullName = "Karel",
                        TwitterUrl= "dsfsdsss", // Invalid url
                    } }
          };
    }
}
