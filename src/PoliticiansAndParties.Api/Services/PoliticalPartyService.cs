namespace PoliticiansAndParties.Api.Services;

public class PoliticalPartyService : IPoliticalPartyService
{
    private readonly ILoggerAdapter<PoliticalPartyService> _logger;
    private readonly IPoliticalPartyRepository _politicalPartyRepository;

    public PoliticalPartyService(
        IPoliticalPartyRepository politicalPartyRepository,
        ILoggerAdapter<PoliticalPartyService> logger)
    {
        _politicalPartyRepository = politicalPartyRepository;
        _logger = logger;
    }

    public async Task<ResultOrFailure<PoliticalParty>> Create(PoliticalParty politicalParty)
    {
        _logger.LogDebug("Creating a political party");

        bool partyExists = await _politicalPartyRepository.ExistsByName(politicalParty.Name);

        if (partyExists)
        {
            _logger.LogWarn("Political party with name {Name} already exists", politicalParty.Name);

            return new Failure($"Political party with name {politicalParty.Name} already exists");
        }

        var createdParty = await _politicalPartyRepository.Create(politicalParty);
        _logger.LogInfo("Political party with id {Id} created", politicalParty.Id);

        return createdParty;
    }

    public async Task<ResultOrNotFound<PoliticalParty>> GetOne(Guid id)
    {
        _logger.LogDebug("Getting political party with id {Id}", id);
        var result = await _politicalPartyRepository.GetOne(id);

        result.Switch(
            _ => _logger.LogDebug("Political party with id {Id} found", id),
            _ => _logger.LogWarn("Political party with id {Id} not found", id));

        return result;
    }

    public async Task<SuccessOrNotFound> Delete(Guid partyId)
    {
        _logger.LogDebug("Deleting party with id {Id}", partyId);
        var result = await _politicalPartyRepository.Delete(partyId);

        result.Switch(
            _ => _logger.LogInfo("Political party with id {Id} deleted", partyId),
            _ => _logger.LogWarn("Unable to delete party with id {Id}, not found", partyId));

        return result;
    }

    public async Task<IEnumerable<PoliticalParty>> GetAll()
    {
        _logger.LogDebug("Fetching all political parties");

        return await _politicalPartyRepository.GetAll();
    }

    public async Task<ResultNotFoundOrFailure<PoliticalParty>> UpdateAsync(PoliticalParty politicalParty)
    {
        _logger.LogDebug("Updating political party with id {Id}", politicalParty.FrontEndId);

        bool partyExists = await _politicalPartyRepository.ExistsByName(politicalParty.Name);

        if (partyExists)
        {
            _logger.LogWarn("Political party with name {Name} already exists", politicalParty.Name);

            return new Failure($"Political party with name {politicalParty.Name} already exists");
        }

        var updateResult = await _politicalPartyRepository.Update(politicalParty);
        return updateResult.Match<ResultNotFoundOrFailure<PoliticalParty>>(
            updatedSuccess =>
            {
                _logger.LogInfo("Political party with id {Id} updated", politicalParty.FrontEndId);

                return updatedSuccess;
            },
            notFound =>
            {
                _logger.LogInfo("Unable to udpate political party with id {Id}, not found", politicalParty.FrontEndId);
                return notFound;
            });
    }
}
