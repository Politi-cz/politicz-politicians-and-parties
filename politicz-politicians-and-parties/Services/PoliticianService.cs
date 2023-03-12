using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Result;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticianService : IPoliticianService
    {
        private readonly IPoliticianRepository _politicianRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IValidator<PoliticianDto> _validator;
        private readonly ILoggerAdapter<PoliticianService> _logger;

        public PoliticianService(IPoliticianRepository politicianRepository, IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticianDto> validator, ILoggerAdapter<PoliticianService> logger)
        {
            _politicianRepository = politicianRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Politician>> CreateAsync(Guid partyId, Politician politician)
        {
            _logger.LogDebug("Creating politician to a political party with id {id}", partyId);

            var internalPartyId = await _politicalPartyRepository.GetInternalIdAsync(partyId);

            if (internalPartyId is null)
            {
                var msg = $"Political party with id {partyId} does not exist";
                _logger.LogWarn("Political party with id {partyId} does not exist", partyId);

                return new Result<Politician>(new ErrorDetail(msg), ErrorType.Invalid);
            }

            politician.PoliticalPartyId = (int)internalPartyId;

            var created = await _politicianRepository.CreateOneAsync(politician);
            _logger.LogInfo("Politician with id {id} created", politician.FrontEndId);

            return new Result<Politician>(created);
        }

        public async Task<bool> DeleteAsync(Guid frontEndId)
        {
            _logger.LogDebug("Deleting party with id {id}", frontEndId);
            var deleted = await _politicianRepository.DeleteAsync(frontEndId);

            if (!deleted)
            {
                _logger.LogWarn("Politician with id {id} not found", frontEndId);
            }
            else
            {
                _logger.LogInfo("Politician with id {id} deleted", frontEndId);
            }

            return deleted;
        }

        public async Task<Result<Politician>> GetAsync(Guid id)
        {
            _logger.LogDebug("Getting politician with id {id}", id);

            var politician = await _politicianRepository.GetAsync(id);

            if (politician is not null)
                return new Result<Politician>(politician);

            _logger.LogWarn("Politician with id {id} not found", id);
            return new Result<Politician>(ErrorType.NotFound);

        }

        public async Task<Result<Politician>> UpdateAsync(Politician politician)
        {
            _logger.LogDebug("Updating politician with id {id}", politician.FrontEndId);

            var updated = await _politicianRepository.UpdateAsync(politician);

            if (!updated)
            {
                _logger.LogWarn("Politician with id {id} not found", politician.FrontEndId);
                return new Result<Politician>(ErrorType.NotFound);
            }
            
            _logger.LogInfo("Politician with id {id} updated", politician.FrontEndId);

            return new Result<Politician>(politician);
        }
    }
}
