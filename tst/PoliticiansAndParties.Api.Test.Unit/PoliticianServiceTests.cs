namespace PoliticiansAndParties.Api.Test.Unit;

public class PoliticianServiceTests
{
    private readonly ILoggerAdapter<PoliticianService> _logger =
        Substitute.For<ILoggerAdapter<PoliticianService>>();

    private readonly IPoliticalPartyRepository _politicalPartyRepository =
        Substitute.For<IPoliticalPartyRepository>();

    private readonly IPoliticianRepository _politicianRepository =
        Substitute.For<IPoliticianRepository>();

    private readonly PoliticianService _sut;

    public PoliticianServiceTests() =>
        _sut = new PoliticianService(_politicianRepository, _politicalPartyRepository, new PoliticianDtoValidator(), _logger);

    public static IEnumerable<object[]> InvalidPoliticianData =>
        new List<object[]>
        {
            new object[] { new PoliticianDto() },
            new object[]
            {
                new PoliticianDto
                {
                    BirthDate = new DateTime(2000, 12, 10),
                    FullName = string.Empty, // Validation error
                    FacebookUrl = "https://facebook.com/test",
                    TwitterUrl = "https://twitter.com/test",
                    InstagramUrl = "https://instagram.com/test",
                },
            },
            new object[]
            {
                new PoliticianDto
                {
                    BirthDate = new DateTime(), // Error
                    FullName = "Karel",
                    FacebookUrl = "https://facebook.com/test",
                    TwitterUrl = "https://twitter.com/test",
                    InstagramUrl = "https://instagram.com/test",
                },
            },
            new object[]
            {
                new PoliticianDto
                {
                    BirthDate = new DateTime(2000, 12, 10),
                    FullName = "Karel",
                    FacebookUrl = "htt://facebook.com/test", // Invalid url
                },
            },
            new object[]
            {
                new PoliticianDto
                {
                    BirthDate = new DateTime(2000, 12, 10),
                    FullName = "Karel",
                    InstagramUrl = "fdsafasd", // Invalid url
                },
            },
            new object[]
            {
                new PoliticianDto
                {
                    BirthDate = new DateTime(2000, 12, 10),
                    FullName = "Karel",
                    TwitterUrl = "dsfsdsss", // Invalid url
                },
            },
        };

    [Fact]
    public async Task GetAsync_ReturnsResult_WhenPoliticianExists()
    {
        // Arrange
        var existingPolitician = new Politician
        {
            Id = 1,
            FrontEndId = Guid.NewGuid(),
            BirthDate = DateTime.Now,
            ImageUrl = "https://newimage.com",
            FullName = "Petr Koller",
            PoliticalPartyId = 25,
        };
        var expected = new Result<Politician>(existingPolitician);
        _ = _politicianRepository.GetAsync(existingPolitician.FrontEndId).Returns(existingPolitician);

        // Act
        var result = await _sut.GetAsync(existingPolitician.FrontEndId);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAsync_ReturnsNotFoundResult_WhenPoliticianDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        _ = _politicianRepository.GetAsync(guid).ReturnsNull();
        var expected = new Result<Politician>(ErrorType.NotFound);

        // Act
        var result = await _sut.GetAsync(guid);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(guid));
    }

    [Fact]
    public async Task CreateAsync_ReturnsErrorResult_WhenParentPoliticalPartyDoesNotExist()
    {
        // Arrange
        var politician = new Politician
        {
            FullName = "Test User",
            BirthDate = DateTime.Now,
            ImageUrl = "https://newimage.com",
            FrontEndId = Guid.NewGuid(),
        };
        var nonExistingPartyId = Guid.NewGuid();

        _ = _politicalPartyRepository.GetInternalIdAsync(nonExistingPartyId).ReturnsNull();
        var expectedResult =
            new Result<Politician>(
                new ErrorDetail($"Political party with id {nonExistingPartyId} does not exist"));

        // Act
        var result = await _sut.CreateAsync(nonExistingPartyId, politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expectedResult);
        _logger.Received(1).LogWarn(
            Arg.Is("Political party with id {partyId} does not exist"),
            Arg.Is(nonExistingPartyId));
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
            ImageUrl = "https://newimage.com",
            InstagramUrl = "https://ig.com/tst",
            Id = 1,
            PoliticalPartyId = 5,
        };

        var expectedResult = new Result<Politician>(politician);
        var politicalPartyGuid = Guid.NewGuid();

        _ = _politicalPartyRepository.GetInternalIdAsync(politicalPartyGuid)
            .Returns(politician.PoliticalPartyId);
        _ = _politicianRepository.CreateOneAsync(Arg.Any<Politician>()).Returns(politician);

        // Act
        var created = await _sut.CreateAsync(politicalPartyGuid, politician);

        // Assert
        _ = created.Should().BeEquivalentTo(expectedResult);
        _logger.Received(1).LogInfo(
            Arg.Is("Politician with id {id} created"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPolitician_WhenDataValid()
    {
        // Arrange
        var politician = new Politician
        {
            BirthDate = DateTime.Now,
            ImageUrl = "https://newimage.com",
            FrontEndId = Guid.NewGuid(),
            FullName = "Test",
        };
        var expected = new Result<Politician>(politician);

        _ = _politicianRepository.UpdateAsync(Arg.Any<Politician>()).Returns(true);

        // Act
        var result = await _sut.UpdateAsync(politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(
            Arg.Is("Politician with id {id} updated"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFoundResult_WhenPoliticianNotFound()
    {
        // Arrange
        var politician = new Politician
        {
            FrontEndId = Guid.NewGuid(),
            ImageUrl = "https://newimage.com",
            BirthDate = DateTime.Now,
            FullName = "Test",
        };
        var expected = new Result<Politician>(ErrorType.NotFound);

        _ = _politicianRepository.UpdateAsync(Arg.Any<Politician>()).Returns(false);

        // Act
        var result = await _sut.UpdateAsync(politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(
            Arg.Is("Politician with id {id} not found"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsValidResult_WhenPoliticianDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianRepository.DeleteAsync(id).Returns(true);
        var expected = new Result<Guid>(id);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(Arg.Is("Politician with id {id} deleted"), Arg.Is(id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFoundResult_WhenPoliticianNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianRepository.DeleteAsync(id).Returns(false);
        var expected = new Result<Guid>(ErrorType.NotFound);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(Arg.Is("Politician with id {id} not found"), Arg.Is(id));
    }
}
