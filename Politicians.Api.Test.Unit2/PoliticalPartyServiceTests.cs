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

namespace PoliticiansAndParties.Api.Test.Unit
{
    public class PoliticalPartyServiceTests
    {
        private readonly PoliticalPartyService _sut;
        private readonly IPoliticalPartyRepository _politicianPartyRepository = Substitute.For<IPoliticalPartyRepository>();
        private readonly ILoggerAdapter<PoliticalPartyService> _logger = Substitute.For<ILoggerAdapter<PoliticalPartyService>>();

        public PoliticalPartyServiceTests()
        {
            _sut = new PoliticalPartyService(_politicianPartyRepository, new PoliticalPartyDtoValidator(), new UpdatePoliticalPartyDtoValidator(), _logger);
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
            var guid = Guid.NewGuid();
            _politicianPartyRepository.GetAsync(guid).ReturnsNull();

            // Act
            var result = await _sut.GetOneAsync(guid);
            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogWarn(Arg.Is("Political party with id {id} not found"), Arg.Is(guid));
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

        [Theory]
        [MemberData(nameof(InvalidPoliticalPartyData))]
        public async Task CreateAsync_ThrowsValidationError_WhenInvalidPoliticianDto(PoliticalPartyDto politicalPartyDto)
        {
            // Arrange
            _politicianPartyRepository.ExistsByNameAsync(Arg.Any<string>()).Returns(false);
            _politicianPartyRepository.CreateAsync(Arg.Any<PoliticalParty>()).Returns(false);

            // Act
            var act = async () => await _sut.CreateAsync(politicalPartyDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenPoliticalPartyWithSameNameExists()
        {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Name = "Existing party",
                ImageUrl = "https://imageurl.com",
                Tags = new HashSet<string> { "Test party" },
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto {
                        BirthDate= DateTime.Now,
                        FullName = "Testing politician",
                    }
                }
            };

            _politicianPartyRepository.ExistsByNameAsync(politicalPartyDto.Name).Returns(true);
            _politicianPartyRepository.CreateAsync(Arg.Any<PoliticalParty>()).Returns(false);

            // Act
            var act = async () => await _sut.CreateAsync(politicalPartyDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Political party with name {politicalPartyDto.Name} already exists");
            _logger.Received(1).LogWarn(Arg.Is($"Political party with name {politicalPartyDto.Name} already exists"));

        }

        [Fact]
        public async Task CreateAsync_CreatePoliticalParty_WhenPoliticalPartyDtoValid()
        {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Name = "New party",
                ImageUrl = "https://imageurl.com",
                Tags = new HashSet<string> { "Test party" },
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto {
                        BirthDate= DateTime.Now,
                        FullName = "Testing politician",
                    }
                }
            };
            var politicalParty = politicalPartyDto.ToPoliticalParty();

            _politicianPartyRepository.ExistsByNameAsync(politicalPartyDto.Name).Returns(false);
            _politicianPartyRepository.CreateAsync(Arg.Do<PoliticalParty>(x => politicalParty = x)).Returns(true);

            // Act
            var result = await _sut.CreateAsync(politicalPartyDto);

            // Assert
            result.Should().BeTrue();
            politicalPartyDto.Id.Should().Be(politicalParty.FrontEndId);
            _logger.Received(1).LogInfo(Arg.Is("Political party with id {id} created"), Arg.Is(politicalParty.FrontEndId));
        }

        [Fact]
        public async Task CreateAsync_ReturnsFalse_WhenPartyNotCreated()
        {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Name = "New party",
                ImageUrl = "https://imageurl.com",
                Tags = new HashSet<string> { "Test party" },
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto {
                        BirthDate= DateTime.Now,
                        FullName = "Testing politician",
                    }
                }
            };

            _politicianPartyRepository.ExistsByNameAsync(politicalPartyDto.Name).Returns(false);
            _politicianPartyRepository.CreateAsync(Arg.Any<PoliticalParty>()).Returns(false);

            // Act
            var result = await _sut.CreateAsync(politicalPartyDto);

            // Assert
            result.Should().BeFalse();
            _logger.Received(1).LogError(null, Arg.Is("Unable to create political party"));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesParty_WhenDataValid()
        {
            // Arrange
            var updatePoliticalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                Name = "updated party",
                ImageUrl = "https://test.com",
                Tags = new HashSet<string> { "test" }
            };

            _politicianPartyRepository.ExistsByNameAsync(updatePoliticalParty.Name).Returns(false);
            _politicianPartyRepository.UpdateAsync(Arg.Any<PoliticalParty>()).Returns(true);

            // Act
            var result = await _sut.UpdateAsync(updatePoliticalParty);

            // Assert
            result.Should().BeTrue();
            _logger.Received(1).LogInfo(Arg.Is("Political party with id {id} updated"), Arg.Is(updatePoliticalParty.Id));

        }

        [Fact]
        public async Task UpdateAsync_ThrowsValidationException_WhenDataInvalid()
        {
            // Arrange
            var updatePoliticalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                Name = "updated party",
                ImageUrl = "https://test.com",
            };
            // Act
            var act = async () => await _sut.UpdateAsync(updatePoliticalParty);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsValidationException_WhenUpdatedPartyNameAlreadyExists()
        {
            // Arrange
            var updatePoliticalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                Name = "updated party",
                ImageUrl = "https://test.com",
                Tags = new HashSet<string> { "test" }
            };

            _politicianPartyRepository.ExistsByNameAsync(updatePoliticalParty.Name).Returns(true);
            _politicianPartyRepository.UpdateAsync(Arg.Any<PoliticalParty>()).Returns(false);
            // Act
            var act = async () => await _sut.UpdateAsync(updatePoliticalParty);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Political party with name {updatePoliticalParty.Name} already exists");
            _logger.Received(1).LogWarn(Arg.Is("Political party with name {name} already exists"), Arg.Is(updatePoliticalParty.Name));

        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenPartyDoesNotExist()
        {
            // Arrange
            var updatePoliticalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                Name = "updated party",
                ImageUrl = "https://test.com",
                Tags = new HashSet<string> { "test" }
            };

            _politicianPartyRepository.ExistsByNameAsync(updatePoliticalParty.Name).Returns(false);
            _politicianPartyRepository.UpdateAsync(Arg.Any<PoliticalParty>()).Returns(false);

            // Act
            var result = await _sut.UpdateAsync(updatePoliticalParty);

            // Assert
            result.Should().BeFalse();
            _logger.Received(1).LogWarn(Arg.Is("Unable to udpate political party with id {id}, not found"), Arg.Is(updatePoliticalParty.Id));
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenPartyDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            _politicianPartyRepository.DeleteAsync(id).Returns(true);

            // Act
            var result = await _sut.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            _logger.Received(1).LogInfo(Arg.Is("Political party with id {id} deleted"), Arg.Is(id));

        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenPartyNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _politicianPartyRepository.DeleteAsync(id).Returns(false);

            // Act
            var result = await _sut.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();
            _logger.Received(1).LogWarn(Arg.Is("Unable to delete party with id {id}, not found"), Arg.Is(id));
        }



        public static IEnumerable<object[]> InvalidPoliticalPartyData =>
          new List<object[]>
          {
                new object[] {
                    new PoliticalPartyDto{} // Empry political party
                },
                new object[]{
                    new PoliticalPartyDto{
                        Name = "Party",
                        ImageUrl = "https://testimageurl.com/",
                        // Tags list empty
                        Politicians = new List<PoliticianDto>{
                            new PoliticianDto
                            {
                                    FullName= "Test",
                                    BirthDate = DateTime.Now,
                            }
                        }
                    }
                },
                new object[]{
                    new PoliticalPartyDto{
                        Name = "Party",
                        ImageUrl = "https://testimageurl.com/",
                        Tags = new HashSet<string> {"test"},
                        // Politicians list empty
                    }
                },
                new object[]{
                    new PoliticalPartyDto{
                        Name = "Party",
                        ImageUrl = "https://testimageurl.com/",
                        Tags = new HashSet<string> {"test"},
                        Politicians = new List<PoliticianDto>{
                            new PoliticianDto // Invalid politician dto
                            {
                                    BirthDate = DateTime.Now,
                            }
                        }
                    }
                },
                new object[]{
                    new PoliticalPartyDto{
                        Name = "", // Empty party name
                        ImageUrl = "https://testimageurl.com/",
                        Tags = new HashSet<string> {"test"},
                        Politicians = new List<PoliticianDto>{
                            new PoliticianDto
                            {
                                    FullName= "Test",
                                    BirthDate = DateTime.Now,
                            }
                        }
                    }
                },
                new object[]{
                    new PoliticalPartyDto{
                        Name = "Test",
                        // missing image url
                        Tags = new HashSet<string> {"test"},
                        Politicians = new List<PoliticianDto>{
                            new PoliticianDto
                            {
                                    FullName= "Test",
                                    BirthDate = DateTime.Now,
                            }
                        }
                    }
                }

          };

    }
}
