using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Validators;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticalPartyService : IPoliticalPartyService
    {
        readonly IPoliticalPartyRepository _politicalPartyRepository;
        readonly IValidator<PoliticalPartyDto> _politicalPartyValidator;
        readonly IValidator<UpdatePoliticalPartyDto> _updatePoliticalPartyValidator;

        readonly ILoggerAdapter<PoliticalPartyService> _logger;

        public PoliticalPartyService(IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticalPartyDto> politicalPartyValidator, IValidator<UpdatePoliticalPartyDto> updatePartyValidator, ILoggerAdapter<PoliticalPartyService> logger)
        {
            _politicalPartyRepository = politicalPartyRepository;
            _politicalPartyValidator = politicalPartyValidator;
            _updatePoliticalPartyValidator = updatePartyValidator;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(PoliticalPartyDto politicalPartyDto)
        {
            _logger.LogDebug("Creating political party");

            _politicalPartyValidator.ValidateAndThrow(politicalPartyDto);

            var partyExists = await _politicalPartyRepository.ExistsByNameAsync(politicalPartyDto.Name);

            if (partyExists)
            {
                var msg = $"Political party with name {politicalPartyDto.Name} already exists";
                _logger.LogWarn(msg);

                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(politicalPartyDto.Name), msg));
            }

            politicalPartyDto.Id = Guid.NewGuid();
            politicalPartyDto.Politicians.ForEach(x => x.Id = Guid.NewGuid());

            var created = await _politicalPartyRepository.CreateAsync(politicalPartyDto.ToPoliticalParty());

            if (created)
            {
                _logger.LogInfo("Political party with id {id} created", politicalPartyDto.Id);
            }
            else
            {
                _logger.LogError(null, "Unable to create political party");
            }

            return created;
        }

        public async Task<bool> DeleteAsync(Guid partyId)
        {
            _logger.LogDebug("Deleting party with id {id}", partyId);
            var deleted = await _politicalPartyRepository.DeleteAsync(partyId);

            if (!deleted)
            {
                _logger.LogWarn("Unable to delete party with id {id}, not found", partyId);

                return false;
            }

            _logger.LogInfo("Political party with id {id} deleted", partyId);
            return true;
        }

        public async Task<IEnumerable<PoliticalPartySideNavDto>> GetAllAsync()
        {
            _logger.LogDebug("Fetching all political parties");
            var politicalParties = await _politicalPartyRepository.GetAllAsync();
            var politicalPartiesSideNav = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

            return politicalPartiesSideNav;
        }

        public async Task<PoliticalPartyDto?> GetOneAsync(Guid id)
        {
            _logger.LogDebug("Getting political party with id {id}", id);
            var politicalParty = await _politicalPartyRepository.GetAsync(id);

            if (politicalParty is null)
            {
                _logger.LogWarn("Political party with id {id} not found", id);
            }

            return politicalParty?.ToPoliticalPartyDto();
        }

        public async Task<bool> UpdateAsync(UpdatePoliticalPartyDto updatePoliticalParty)
        {
            _logger.LogDebug("Updating political party with id {id}", updatePoliticalParty.Id);

            _updatePoliticalPartyValidator.ValidateAndThrow(updatePoliticalParty);

            var partyExists = await _politicalPartyRepository.ExistsByNameAsync(updatePoliticalParty.Name);

            if (partyExists)
            {
                _logger.LogWarn("Political party with name {name} already exists", updatePoliticalParty.Name);

                var msg = $"Political party with name {updatePoliticalParty.Name} already exists";
                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(updatePoliticalParty.Name), msg));
            }

            var politicalParty = updatePoliticalParty.ToPoliticalParty();

            var updated = await _politicalPartyRepository.UpdateAsync(politicalParty);

            if (!updated)
            {
                _logger.LogWarn("Unable to udpate political party with id {id}, not found", updatePoliticalParty.Id);
                return updated;
            }

            _logger.LogInfo("Political party with id {id} updated", updatePoliticalParty.Id);
            return updated;
        }
    }
}
