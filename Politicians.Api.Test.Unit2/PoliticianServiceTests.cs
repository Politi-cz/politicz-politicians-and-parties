using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Contracts.Requests;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Result;
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
        public async Task GetAsync_ReturnsResult_WhenPoliticianExists()
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
            var expected = new Result<Politician>(existingPolitician);
            _politicianRepository.GetAsync(existingPolitician.FrontEndId).Returns(existingPolitician);

            // Act
            var result = await _sut.GetAsync(existingPolitician.FrontEndId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFoundResult_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _politicianRepository.GetAsync(guid).ReturnsNull();
            var expected = new Result<Politician>(ErrorType.NotFound);

            // Act
            var result = await _sut.GetAsync(guid);

            // Assert
            result.Should().BeEquivalentTo(expected);
            _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(guid));

        }


        [Fact]
        public async Task CreateAsync_ReturnsErrorResult_WhenParentPoliticalPartyDoesNotExist()
        {
            // Arrange
            var politician = new Politician()
            {
                FullName = "Test User",
                BirthDate = DateTime.Now,
                FrontEndId = Guid.NewGuid()
            };
            var nonExistingPartyId = Guid.NewGuid();

            _politicalPartyRepository.GetInternalIdAsync(nonExistingPartyId).ReturnsNull();
            var expectedResult = new Result<Politician>(new ErrorDetail($"Political party with id {nonExistingPartyId} does not exist"));

            // Act
            var result = await _sut.CreateAsync(nonExistingPartyId, politician);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _logger.Received(1).LogWarn(Arg.Is("Political party with id {partyId} does not exist"), Arg.Is(nonExistingPartyId));

        }

        [Fact]
        public async Task CreateAsync_CreatesPolitician_WhenDataValid()
        {
            // Arrange
            var politician = new Politician
            {
                BirthDate = DateTime.Now,
                FrontEndId = Guid.NewGuid(),
                FullName = "Test Name",
                InstagramUrl = "https://ig.com/tst",
                Id = 1,
                PoliticalPartyId = 5
            };

            var expectedResult = new Result<Politician>(politician);
            var politicalPartyGuid = Guid.NewGuid();

            _politicalPartyRepository.GetInternalIdAsync(politicalPartyGuid).Returns(politician.PoliticalPartyId);
            _politicianRepository.CreateOneAsync(Arg.Any<Politician>()).Returns(politician);

            // Act
            var created = await _sut.CreateAsync(politicalPartyGuid, politician);

            // Assert
            created.Should().BeEquivalentTo(expectedResult);
            _logger.Received(1).LogInfo(Arg.Is("Politician with id {id} created"), Arg.Is(politician.FrontEndId));

        }

        [Fact]
        public async Task UpdateAsync_UpdatesPolitician_WhenDataValid()
        {
            // Arrange
            var politician = new Politician
            {
                BirthDate = DateTime.Now,
                FrontEndId = Guid.NewGuid(),
                FullName = "Test"
            };
            var expected = new Result<Politician>(politician);
            
            _politicianRepository.UpdateAsync(Arg.Any<Politician>()).Returns(true);
            
            // Act
            var result = await _sut.UpdateAsync(politician);

            // Assert
            result.Should().BeEquivalentTo(expected);
            _logger.Received(1).LogInfo(Arg.Is("Politician with id {id} updated"), Arg.Is(politician.FrontEndId));
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFoundResult_WhenPoliticianNotFound()
        {
            // Arrange
            var politician = new Politician
            {
                FrontEndId = Guid.NewGuid(), 
                BirthDate = DateTime.Now,
                FullName = "Test",
            };
            var expected = new Result<Politician>(ErrorType.NotFound);
            
            _politicianRepository.UpdateAsync(Arg.Any<Politician>()).Returns(false);

            // Act
            var result = await _sut.UpdateAsync(politician);

            // Assert
            result.Should().BeEquivalentTo(expected);
            _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(politician.FrontEndId));
        }
        
        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenPoliticianDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            _politicianRepository.DeleteAsync(id).Returns(true);

            // Act
            var result = await _sut.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            _logger.Received(1).LogInfo(Arg.Is("Politician with id {id} deleted"), Arg.Is(id));
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenPoliticianNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _politicianRepository.DeleteAsync(id).Returns(false);

            // Act
            var result = await _sut.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();
            _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(id));
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
