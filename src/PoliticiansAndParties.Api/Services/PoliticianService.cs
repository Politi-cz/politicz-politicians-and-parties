namespace PoliticiansAndParties.Api.Services;

public class PoliticianService : IPoliticianService
{
    private readonly ILoggerAdapter<PoliticianService> _logger;
    private readonly IPoliticalPartyRepository _politicalPartyRepository;
    private readonly IPoliticianRepository _politicianRepository;

    public PoliticianService(
        IPoliticianRepository politicianRepository,
        IPoliticalPartyRepository politicalPartyRepository,
        ILoggerAdapter<PoliticianService> logger)
    {
        _politicianRepository = politicianRepository;
        _politicalPartyRepository = politicalPartyRepository;
        _logger = logger;
    }

    public async Task<ResultOrFailure<Politician>> Create(Guid partyId, Politician politician)
    {
        _logger.LogDebug("Creating politician to a political party with id {Id}", partyId);

        var getIdResult = await _politicalPartyRepository.GetInternalId(partyId);

        if (getIdResult.TryPickT1(out _, out int internalId))
        {
            _logger.LogWarn("Political party with id {partyId} does not exist", partyId);

            return new Failure($"Political party with id {partyId} does not exist");
        }

        politician.PoliticalPartyId = internalId;

        var created = await _politicianRepository.CreateOne(politician);
        _logger.LogInfo("Politician with id {Id} created", politician.FrontEndId);

        return created;
    }

    public async Task<SuccessOrNotFound> Delete(Guid frontEndId)
    {
        _logger.LogDebug("Deleting party with id {Id}", frontEndId);
        var result = await _politicianRepository.Delete(frontEndId);

        result.Switch(
            _ => _logger.LogInfo("Politician with id {Id} deleted", frontEndId),
            _ => _logger.LogWarn("Politician with id {Id} not found", frontEndId));

        return result;
    }

    public async Task<ResultOrNotFound<Politician>> Get(Guid id)
    {
        _logger.LogDebug("Getting politician with id {Id}", id);

        var result = await _politicianRepository.Get(id);

        result.Switch(
            _ => _logger.LogDebug("Got politician with id {Id}", id),
            _ => _logger.LogWarn("Politician with id {Id} not found", id));

        return result;
    }

    public async Task<ResultOrNotFound<Politician>> Update(Politician politician)
    {
        _logger.LogDebug("Updating politician with id {Id}", politician.FrontEndId);

        var result = await _politicianRepository.Update(politician);

        result.Switch(
            _ => _logger.LogInfo("Politician with id {Id} updated", politician.FrontEndId),
            _ => _logger.LogWarn("Politician with id {Id} not found", politician.FrontEndId));

        return result;
    }
}
