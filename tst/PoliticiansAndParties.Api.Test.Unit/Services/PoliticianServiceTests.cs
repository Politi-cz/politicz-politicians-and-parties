namespace PoliticiansAndParties.Api.Test.Unit.Services;

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
        _sut = new PoliticianService(_politicianRepository, _politicalPartyRepository, _logger);

    [Fact]
    public async Task Get_ReturnsResult_WhenPoliticianExists()
    {
        // Arrange
        var existingPolitician = TestData.GetPolitician;
        var expected = new ResultOrNotFound<Politician>(existingPolitician);
        _ = _politicianRepository.Get(existingPolitician.FrontEndId).Returns(existingPolitician);

        // Act
        var result = await _sut.Get(existingPolitician.FrontEndId);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenPoliticianDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        _ = _politicianRepository.Get(guid).Returns(default(NotFound));
        var expected = new ResultOrNotFound<Politician>(default(NotFound));

        // Act
        var result = await _sut.Get(guid);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(Arg.Is("Politician with id {Id} not found"), Arg.Is(guid));
    }

    [Fact]
    public async Task Create_ReturnsFailure_WhenParentPoliticalPartyDoesNotExist()
    {
        // Arrange
        var politician = TestData.GetPolitician;
        var nonExistingPartyId = Guid.NewGuid();

        _ = _politicalPartyRepository.GetInternalId(nonExistingPartyId).Returns(default(NotFound));
        var expectedResult =
            new ResultOrFailure<Politician>(
                new Failure($"Political party with id {nonExistingPartyId} does not exist"));

        // Act
        var result = await _sut.Create(nonExistingPartyId, politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expectedResult);
        _logger.Received(1).LogWarn(
            Arg.Is("Political party with id {PartyId} does not exist"),
            Arg.Is(nonExistingPartyId));
    }

    [Fact]
    public async Task Create_CreatesPolitician_WhenDataValid()
    {
        // Arrange
        var politician = TestData.GetPolitician;

        var expectedResult = new ResultOrFailure<Politician>(politician);
        var politicalPartyGuid = Guid.NewGuid();

        _ = _politicalPartyRepository.GetInternalId(politicalPartyGuid)
            .Returns(politician.PoliticalPartyId);
        _ = _politicianRepository.CreateOne(Arg.Any<Politician>()).Returns(politician);

        // Act
        var created = await _sut.Create(politicalPartyGuid, politician);

        // Assert
        _ = created.Should().BeEquivalentTo(expectedResult);
        _logger.Received(1).LogInfo(
            Arg.Is("Politician with id {Id} created"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task Update_UpdatesPolitician_WhenDataValid()
    {
        // Arrange
        var politician = TestData.GetPolitician;
        var expected = new ResultOrNotFound<Politician>(politician);

        _ = _politicianRepository.Update(Arg.Any<Politician>()).Returns(politician);

        // Act
        var result = await _sut.Update(politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(
            Arg.Is("Politician with id {Id} updated"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenPoliticianNotFound()
    {
        // Arrange
        var politician = TestData.GetPolitician;
        var expected = new ResultOrNotFound<Politician>(default(NotFound));

        _ = _politicianRepository.Update(Arg.Any<Politician>()).Returns(default(NotFound));

        // Act
        var result = await _sut.Update(politician);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(
            Arg.Is("Politician with id {Id} not found"),
            Arg.Is(politician.FrontEndId));
    }

    [Fact]
    public async Task Delete_ReturnsSuccess_WhenPoliticianDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianRepository.Delete(id).Returns(default(Success));
        var expected = new SuccessOrNotFound(default(Success));

        // Act
        var result = await _sut.Delete(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(Arg.Is("Politician with id {Id} deleted"), Arg.Is(id));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPoliticianNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianRepository.Delete(id).Returns(default(NotFound));
        var expected = new SuccessOrNotFound(default(NotFound));

        // Act
        var result = await _sut.Delete(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(Arg.Is("Politician with id {Id} not found"), Arg.Is(id));
    }
}
