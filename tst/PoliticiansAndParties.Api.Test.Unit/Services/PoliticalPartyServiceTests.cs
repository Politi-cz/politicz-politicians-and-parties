namespace PoliticiansAndParties.Api.Test.Unit.Services;

public class PoliticalPartyServiceTests
{
    private readonly ILoggerAdapter<PoliticalPartyService> _logger =
        Substitute.For<ILoggerAdapter<PoliticalPartyService>>();

    private readonly IPoliticalPartyRepository _politicianPartyRepository =
        Substitute.For<IPoliticalPartyRepository>();

    private readonly PoliticalPartyService _sut;

    public PoliticalPartyServiceTests() => _sut = new PoliticalPartyService(_politicianPartyRepository, _logger);

    [Fact]
    public async Task GetOne_ReturnsPoliticalPartyDto_WhenPartyExists()
    {
        // Arrange
        var politicalParty = TestData.GetPoliticalParty;
        _ = _politicianPartyRepository.GetOne(politicalParty.FrontEndId).Returns(politicalParty);
        var expected = new ResultOrNotFound<PoliticalParty>(politicalParty);

        // Act
        var result = await _sut.GetOne(politicalParty.FrontEndId);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetOne_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var expected = new ResultOrNotFound<PoliticalParty>(default(NotFound));
        _ = _politicianPartyRepository.GetOne(Arg.Any<Guid>()).Returns(default(NotFound));

        // Act
        var result = await _sut.GetOne(guid);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(Arg.Is("Political party with id {Id} not found"), Arg.Is(guid));
    }

    [Fact]
    public async Task GetAll_ReturnsPoliticalParties_WhenPartiesExist()
    {
        // Arrange
        var politicalParties = TestData.GetPoliticalParties;
        _ = _politicianPartyRepository.GetAll().Returns(politicalParties);

        // Act
        var result = await _sut.GetAll();

        // Assert
        _ = result.Should().BeEquivalentTo(politicalParties);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyIEnumerable_WhenPartyDoesNotExist()
    {
        // Arrange
        _ = _politicianPartyRepository.GetAll().Returns(Enumerable.Empty<PoliticalParty>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        _ = result.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_ReturnsFailure_WhenPoliticalPartyWithSameNameExists()
    {
        // Arrange
        var politicalParty = TestData.GetPoliticalParty;
        _ = _politicianPartyRepository.ExistsByName(politicalParty.Name).Returns(true);
        _ = _politicianPartyRepository.Create(Arg.Any<PoliticalParty>()).Returns(politicalParty);
        var expected = new ResultOrFailure<PoliticalParty>(
            new Failure($"Political party with name {politicalParty.Name} already exists"));

        // Act
        var result = await _sut.Create(politicalParty);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1)
            .LogWarn(Arg.Is("Political party with name {Name} already exists"), Arg.Is(politicalParty.Name));
    }

    [Fact]
    public async Task Create_CreatesPoliticalParty_WhenPartyDoesNotExist()
    {
        // Arrange
        var politicalParty = TestData.GetPoliticalParty;
        _ = _politicianPartyRepository.ExistsByName(politicalParty.Name).Returns(false);
        _ = _politicianPartyRepository.Create(Arg.Any<PoliticalParty>()).Returns(politicalParty);
        var expected = new ResultOrFailure<PoliticalParty>(politicalParty);

        // Act
        var result = await _sut.Create(politicalParty);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(
            Arg.Is("Political party with id {Id} created"),
            Arg.Is(politicalParty.FrontEndId));
    }

    [Fact]
    public async Task Update_UpdatesParty_WhenDataValid()
    {
        // Arrange
        var updatePoliticalParty = TestData.GetPoliticalParty;

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(false);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(updatePoliticalParty);
        var expected = new ResultNotFoundOrFailure<PoliticalParty>(updatePoliticalParty);

        // Act
        var result = await _sut.Update(updatePoliticalParty);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(
            Arg.Is("Political party with id {Id} updated"),
            Arg.Is(updatePoliticalParty.FrontEndId));
    }

    [Fact]
    public async Task Update_ReturnsFailure_WhenUpdatedPartyNameAlreadyExists()
    {
        // Arrange
        var updatePoliticalParty = TestData.GetPoliticalParty;

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(true);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(updatePoliticalParty);
        var expected = new ResultNotFoundOrFailure<PoliticalParty>(
            new Failure($"Political party with name {updatePoliticalParty.Name} already exists"));

        // Act
        var result = await _sut.Update(updatePoliticalParty);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(
            Arg.Is("Political party with name {Name} already exists"),
            Arg.Is(updatePoliticalParty.Name));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        // Arrange
        var updatePoliticalParty = TestData.GetPoliticalParty;

        _ = _politicianPartyRepository.ExistsByName(updatePoliticalParty.Name).Returns(false);
        _ = _politicianPartyRepository.Update(Arg.Any<PoliticalParty>()).Returns(default(NotFound));
        var expected = new ResultNotFoundOrFailure<PoliticalParty>(default(NotFound));

        // Act
        var result = await _sut.Update(updatePoliticalParty);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(
            Arg.Is("Unable to update political party with id {Id}, not found"),
            Arg.Is(updatePoliticalParty.FrontEndId));
    }

    [Fact]
    public async Task Delete_ReturnsSuccess_WhenPartyDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianPartyRepository.Delete(id).Returns(default(Success));
        var expected = new SuccessOrNotFound(default(Success));

        // Act
        var result = await _sut.Delete(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogInfo(Arg.Is("Political party with id {Id} deleted"), Arg.Is(id));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPartyNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ = _politicianPartyRepository.Delete(id).Returns(default(NotFound));
        var expected = new SuccessOrNotFound(default(NotFound));

        // Act
        var result = await _sut.Delete(id);

        // Assert
        _ = result.Should().BeEquivalentTo(expected);
        _logger.Received(1).LogWarn(
            Arg.Is("Unable to delete party with id {Id}, not found"),
            Arg.Is(id));
    }
}
