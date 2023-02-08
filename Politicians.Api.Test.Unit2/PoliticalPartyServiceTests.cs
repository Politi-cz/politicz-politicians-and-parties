using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Dtos;
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
        private readonly ILogger<PoliticalPartyService> _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<PoliticalPartyService>>();

        public PoliticalPartyServiceTests()
        {
            _sut = new PoliticalPartyService(_politicianPartyRepository, new PoliticalPartyDtoValidator(), _logger);
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

            // FIX THIS BY CREATING LOG ADAPTER, THEN IT SHOULD WORK
            // _logger.Received(1).Log(LogLevel.Error, "Testing error log {test}", Arg.Any<string>());
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
        public async Task CreateAsync_ShouldThrowValidationError_WhenInvalidPoliticianDto(PoliticalPartyDto politicalPartyDto)
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
        public async Task CreateAsync_ShouldThrowValidationException_WhenPoliticalPartyWithSameNameExists()
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
        }

        [Fact]
        public async Task CreateAsync_ShouldNotCreateParty_WhenPoliticalPartyDtoInvalid()
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
