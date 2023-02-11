using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using politicz_politicians_and_parties.Validators;

namespace Politicians.Api.Test.Unit
{
    public class PoliticianServiceTests
    {
        private readonly PoliticianService _sut;
        private readonly IPoliticianRepository _politicianRepository = Substitute.For<IPoliticianRepository>();
        private readonly IPoliticalPartyRepository _politicalPartyRepository = Substitute.For<IPoliticalPartyRepository>();
        private readonly ILoggerAdapter<PoliticianService> _logger = Substitute.For<ILoggerAdapter<PoliticianService>>();

        public PoliticianServiceTests()
        {
            _sut = new PoliticianService(_politicianRepository, _politicalPartyRepository, new PoliticianDtoValidator(), _logger);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPoliticianDto_WhenPoliticianExists()
        {
            // Arrange
            var existingPolitician = new Politician
            {
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
        public async Task GetAsync_ShouldReturnNull_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _politicianRepository.GetAsync(guid).ReturnsNull();

            // Act
            var result = await _sut.GetAsync(guid);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(guid.ToString()));

        }

        [Theory]
        [MemberData(nameof(InvalidPoliticianData))]
        public async Task CreateAsync_ShouldThrowValidationError_WhenInvalidPoliticianDto(PoliticianDto politicianDto)
        {
            // Arrange
            _politicianRepository.CreateOneAsync(politicianDto.ToPolitician()).Returns(false);
            _politicalPartyRepository.GetInternalIdAsync(Arg.Any<Guid>()).Returns(10);
            // does not really matter

            // Act
            var act = async () => await _sut.CreateAsync(Guid.NewGuid(), politicianDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationError_WhenParentPoliticalPartyDoesNotExist()
        {
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
            _logger.Received(1).LogWarn(Arg.Is($"Political party with id {nonExistingPartyId} does not exist"));

        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePolitician_WhenPoliticianDtoValid()
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
            _logger.Received(1).LogInfo(Arg.Is("Politician with internal id {internalId} created"), Arg.Any<int>());

        }

        [Fact]
        public async Task CreateAsync_ShouldNotCreatePolitician_WhenPoliticianDtoInvalid()
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
