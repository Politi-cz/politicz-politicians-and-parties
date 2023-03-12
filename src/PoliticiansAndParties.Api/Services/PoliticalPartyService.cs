namespace PoliticiansAndParties.Api.Services;

public class PoliticalPartyService : IPoliticalPartyService
{
    private readonly ILoggerAdapter<PoliticalPartyService> _logger;
    private readonly IPoliticalPartyRepository _politicalPartyRepository;
    private readonly IValidator<UpdatePoliticalPartyDto> _updatePoliticalPartyValidator;

    public PoliticalPartyService(
        IPoliticalPartyRepository politicalPartyRepository,
        IValidator<UpdatePoliticalPartyDto> updatePartyValidator,
        ILoggerAdapter<PoliticalPartyService> logger)
    {
        _politicalPartyRepository = politicalPartyRepository;
        _updatePoliticalPartyValidator = updatePartyValidator;
        _logger = logger;
    }

    public async Task<Result<PoliticalParty>> CreateAsync(PoliticalParty politicalParty)
    {
        _logger.LogDebug("Creating a political party");

        bool partyExists = await _politicalPartyRepository.ExistsByNameAsync(politicalParty.Name);

        if (partyExists)
        {
            _logger.LogWarn("Political party with name {name} already exists", politicalParty.Name);

            return new Result<PoliticalParty>(
                new ErrorDetail($"Political party with name {politicalParty.Name} already exists"));
        }

        var createdParty = await _politicalPartyRepository.CreateAsync(politicalParty);
        _logger.LogInfo("Political party with id {id} created", politicalParty.Id);

        return new Result<PoliticalParty>(createdParty);
    }

    public async Task<Result<PoliticalParty>> GetOneAsync(Guid id)
    {
        _logger.LogDebug("Getting political party with id {id}", id);
        var politicalParty = await _politicalPartyRepository.GetAsync(id);

        if (politicalParty is null)
        {
            _logger.LogWarn("Political party with id {id} not found", id);

            return new Result<PoliticalParty>(
                new ErrorDetail($"Political party with id {id} not found"),
                ErrorType.NotFound);
        }

        return new Result<PoliticalParty>(politicalParty);
    }

    public async Task<Result<Guid>> DeleteAsync(Guid partyId)
    {
        _logger.LogDebug("Deleting party with id {id}", partyId);
        bool deleted = await _politicalPartyRepository.DeleteAsync(partyId);

        if (!deleted)
        {
            _logger.LogWarn("Unable to delete party with id {id}, not found", partyId);

            return new Result<Guid>(
                new ErrorDetail($"Political party with id {partyId} not found"),
                ErrorType.NotFound);
        }

        _logger.LogInfo("Political party with id {id} deleted", partyId);
        return new Result<Guid>(partyId);
    }

    public async Task<Result<IEnumerable<PoliticalParty>>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all political parties");
        var politicalParties = await _politicalPartyRepository.GetAllAsync();

        return new Result<IEnumerable<PoliticalParty>>(politicalParties);
    }

    public async Task<bool> UpdateAsync(UpdatePoliticalPartyDto updatePoliticalParty)
    {
        _logger.LogDebug("Updating political party with id {id}", updatePoliticalParty.Id);

        _updatePoliticalPartyValidator.ValidateAndThrow(updatePoliticalParty);

        bool partyExists = await _politicalPartyRepository.ExistsByNameAsync(updatePoliticalParty.Name);

        if (partyExists)
        {
            _logger.LogWarn("Political party with name {name} already exists", updatePoliticalParty.Name);

            string msg = $"Political party with name {updatePoliticalParty.Name} already exists";
            throw new ValidationException(
                msg,
                HelperValidatorMethods.GenerateValidationError(nameof(updatePoliticalParty.Name), msg));
        }

        var politicalParty = updatePoliticalParty.ToPoliticalParty();

        bool updated = await _politicalPartyRepository.UpdateAsync(politicalParty);

        if (!updated)
        {
            _logger.LogWarn("Unable to udpate political party with id {id}, not found", updatePoliticalParty.Id);
            return updated;
        }

        _logger.LogInfo("Political party with id {id} updated", updatePoliticalParty.Id);
        return updated;
    }
}
