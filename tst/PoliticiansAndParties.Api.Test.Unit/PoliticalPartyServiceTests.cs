namespace PoliticiansAndParties.Api.Test.Unit;

public class PoliticalPartyServiceTests
{
    private readonly ILoggerAdapter<PoliticalPartyService> _logger =
        Substitute.For<ILoggerAdapter<PoliticalPartyService>>();

    private readonly IPoliticalPartyRepository _politicianPartyRepository =
        Substitute.For<IPoliticalPartyRepository>();

    private readonly PoliticalPartyService _sut;

    public PoliticalPartyServiceTests() =>
        _sut = new PoliticalPartyService(
            _politicianPartyRepository,
            new PoliticalPartyDtoValidator(),
            new UpdatePoliticalPartyDtoValidator(), _logger);

    public static IEnumerable<object[]> InvalidPoliticalPartyData =>
        new List<object[]>
        {
            new object[]
            {
                new PoliticalPartyDto(), // Empry political party
            },
            new object[]
            {
                new PoliticalPartyDto
                {
                    Name = "Party",
                    ImageUrl = "https://testimageurl.com/",

                    // Tags list empty
                    Politicians = new List<PoliticianDto>
                    {
                        new() { FullName = "Test", BirthDate = DateTime.Now },
                    },
                },
            },
            new object[]
            {
                new PoliticalPartyDto
                {
                    Name = "Party",
                    ImageUrl = "https://testimageurl.com/",
                    Tags = new HashSet<string> { "test" },

                    // Politicians list empty
                },
            },
            new object[]
            {
                new PoliticalPartyDto
                {
                    Name = "Party",
                    ImageUrl = "https://testimageurl.com/",
                    Tags = new HashSet<string> { "test" },
                    Politicians = new List<PoliticianDto>
                    {
                        new() // Invalid politician dto
                        {
                            BirthDate = DateTime.Now,
                        },
                    },
                },
            },
            new object[]
            {
                new PoliticalPartyDto
                {
                    Name = string.Empty, // Empty party name
                    ImageUrl = "https://testimageurl.com/",
                    Tags = new HashSet<string> { "test" },
                    Politicians = new List<PoliticianDto>
                    {
                        new() { FullName = "Test", BirthDate = DateTime.Now },
                    },
                },
            },
            new object[]
            {
                new PoliticalPartyDto
                {
                    Name = "Test",

                    // missing image url
                    Tags = new HashSet<string> { "test" },
                    Politicians = new List<PoliticianDto>
                    {
                        new() { FullName = "Test", BirthDate = DateTime.Now },
                    },
                },
            },
        };

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
            Politicians = new List<Politician>
            {
                new()
                {
                    Id = 1,
                    ImageUrl = string.Empty,
                    FrontEndId = Guid.NewGuid(),
                    BirthDate = DateTime.Now,
                    InstagramUrl = "ig.com/pepaKarel",
                    FullName = "Pepa Karel",
                    PoliticalPartyId = 1,
                },
            },
        };

        var expectedResult = politicalParty.ToPoliticalPartyDto();

        _ = _politicianPartyRepository.GetOne(politicalParty.FrontEndId).Returns(politicalParty);

        // Act
        var result = await _sut.GetOne(politicalParty.FrontEndId);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetOneAsync_ReturnsNull_WhenPartyDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        _politicianPartyRepository.GetOne(guid).ReturnsNull();

        // Act
        var result = await _sut.GetOne(guid);

        // Assert
        _ = result.Should().BeNull();
        _logger.Received(1).LogWarn(Arg.Is("Political party with id {id} not found"), Arg.Is(guid));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEnumerableOfSideNavDto_WhenPartiesExist()
    {
        // Arrange
        var politicalParties = new List<PoliticalParty>
        {
            new()
            {
                Id = 1,
                FrontEndId = Guid.NewGuid(),
                ImageUrl = "img.com",
                Name = "Spolu",
                Politicians = new List<Politician>
                {
                    new()
                    {
                        Id = 1,
                        ImageUrl = string.Empty,
                        FrontEndId = Guid.NewGuid(),
                        BirthDate = DateTime.Now,
                        InstagramUrl = "ig.com/pepaKarel",
                        FullName = "Pepa Karel",
                        PoliticalPartyId = 1,
                    },
                },
            },
            new()
            {
                Id = 2, FrontEndId = Guid.NewGuid(), ImageUrl = "img.com/2", Name = "Teeest",
            },
        };

        var expectedResult = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

        _ = _politicianPartyRepository.GetAll().Returns(politicalParties);

        // Act
        var result = await _sut.GetAll();

        // Assert
        _ = result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetPoliticalPartiesAsync_ReturnsEmptyIEnumerable_WhenPartyDoesNotExist()
    {
        // Arrange
        _ = _politicianPartyRepository.GetAll().Returns(Enumerable.Empty<PoliticalParty>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidPoliticalPartyData))]
    public async Task CreateAsync_ThrowsValidationError_WhenInvalidPoliticianDto(
        PoliticalPartyDto politicalPartyDto)
    {
        // Arrange
        _ = _politicianPartyRepository.ExistsByName(Arg.Any<string>()).Returns(false);
        _politicianPartyRepository.Create(Arg.Any<PoliticalParty>()).Returns(false);

        // Act
        var act = async () => await _sut.Create(politicalPartyDto);

        // Assert
        _ = await act.Should().ThrowAsync<ValidationException>();
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
            Politicians = new List<PoliticianDto>
            {
                new() { BirthDate = DateTime.Now, FullName = "Testing politician" },
            },
        };

        _ = _politicianPartyRepository.ExistsByName(politicalPartyDto.Name).Returns(true);
        _politicianPartyRepository.Create(Arg.Any<PoliticalParty>()).Returns(false);

        // Act
        var act = async () => await _sut.Create(politicalPartyDto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"Political party with name {politicalPartyDto.Name} already exists");
        _logger.Received(1)
            .LogWarn(Arg.Is($"Political party with name {politicalPartyDto.Name} already exists"));
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
            Politicians = new List<PoliticianDto>
            {
                new() { BirthDate = DateTime.Now, FullName = "Testing politician" },
            },
        };
        var politicalParty = politicalPartyDto.ToPoliticalParty();

        _ = _politicianPartyRepository.ExistsByName(politicalPartyDto.Name).Returns(false);
        _politicianPartyRepository.Create(Arg.Do<PoliticalParty>(x => politicalParty = x))
            .Returns(true);

        // Act
        var result = await _sut.Create(politicalPartyDto);

        // Assert
        result.Should().BeTrue();
        politicalPartyDto.Id.Should().Be(politicalParty.FrontEndId);
        _logger.Received(1).LogInfo(
            Arg.Is("Political party with id {id} created"),
            Arg.Is(politicalParty.FrontEndId));
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
            Politicians = new List<PoliticianDto>
            {
                new() { BirthDate = DateTime.Now, FullName = "Testing politician" },
            },
        };

        _ = _politicianPartyRepository.ExistsByName(politicalPartyDto.Name).Returns(false);
        _politicianPartyRepository.Create(Arg.Any<PoliticalParty>()).Returns(false);

        // Act
        var result = await _sut.Create(politicalPartyDto);

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
            Tags = new HashSet<string> { "test" },
        };

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(false);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(true);

        // Act
        var result = await _sut.UpdateAsync(updatePoliticalParty);

        // Assert
        result.Should().BeTrue();
        _logger.Received(1).LogInfo(
            Arg.Is("Political party with id {id} updated"),
            Arg.Is(updatePoliticalParty.Id));
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
        _ = await act.Should().ThrowAsync<ValidationException>();
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
            Tags = new HashSet<string> { "test" },
        };

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(true);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(false);

        // Act
        var act = async () => await _sut.UpdateAsync(updatePoliticalParty);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"Political party with name {updatePoliticalParty.Name} already exists");
        _logger.Received(1).LogWarn(
            Arg.Is("Political party with name {name} already exists"),
            Arg.Is(updatePoliticalParty.Name));
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
            Tags = new HashSet<string> { "test" },
        };

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(false);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(false);

        // Act
        var result = await _sut.UpdateAsync(updatePoliticalParty);

        // Assert
        result.Should().BeFalse();
        _logger.Received(1).LogWarn(
            Arg.Is("Unable to udpate political party with id {id}, not found"),
            Arg.Is(updatePoliticalParty.Id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenPartyDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianPartyRepository.Delete(id).Returns(true);

        // Act
        var result = await _sut.Delete(id);

        // Assert
        result.Should().BeTrue();
        _logger.Received(1).LogInfo(Arg.Is("Political party with id {id} deleted"), Arg.Is(id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenPartyNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianPartyRepository.Delete(id).Returns(false);

        // Act
        var result = await _sut.Delete(id);

        // Assert
        result.Should().BeFalse();
        _logger.Received(1).LogWarn(
            Arg.Is("Unable to delete party with id {id}, not found"),
            Arg.Is(id));
    }
}
