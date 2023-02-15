using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Validators;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticianService : IPoliticianService
    {
        readonly IPoliticianRepository _politicianRepository;
        readonly IPoliticalPartyRepository _politicalPartyRepository;
        readonly IValidator<PoliticianDto> _validator;
        readonly ILoggerAdapter<PoliticianService> _logger;

        public PoliticianService(IPoliticianRepository politicianRepository, IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticianDto> validator, ILoggerAdapter<PoliticianService> logger)
        {
            _politicianRepository = politicianRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(Guid partyId, PoliticianDto politicianDto)
        {
            _logger.LogDebug("Creating politician to a political party with id {id}", partyId);

            _validator.ValidateAndThrow(politicianDto);

            politicianDto.Id = Guid.NewGuid();
            var politician = politicianDto.ToPolitician();

            var internalPartyId = await _politicalPartyRepository.GetInternalIdAsync(partyId);

            if (internalPartyId is null)
            {
                var msg = $"Political party with id {partyId} does not exist";
                _logger.LogWarn(msg);

                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(politicianDto.Id), msg));
            }

            politician.PoliticalPartyId = (int)internalPartyId;

            _logger.LogInfo("Politician with internal id {internalId} created", politician.PoliticalPartyId);

            return await _politicianRepository.CreateOneAsync(politician);
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

        public async Task<PoliticianDto?> GetAsync(Guid id)
        {
            _logger.LogDebug("Getting politician with id {id}", id);

            var politician = await _politicianRepository.GetAsync(id);

            if (politician is null)
            {
                _logger.LogWarn("Politician with id {id} not found", id);
            }

            return politician?.ToPoliticianDto();
        }

        public async Task<bool> UpdateAsync(Guid frontEndId, PoliticianDto politicianDto)
        {
            _logger.LogDebug("Updating politician with id {id}", frontEndId);

            _validator.ValidateAndThrow(politicianDto);

            politicianDto.Id = frontEndId;

            var politician = politicianDto.ToPolitician();

            var updated = await _politicianRepository.UpdateAsync(politician);

            if (!updated)
            {
                _logger.LogWarn("Politician with id {id} not found", frontEndId);
            }
            else
            {
                _logger.LogInfo("Politician with id {id} updated", frontEndId);
            }

            return updated;
        }
    }
}
