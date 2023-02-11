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
        readonly IValidator<PoliticalPartyDto> _validator;
        readonly ILoggerAdapter<PoliticalPartyService> _logger;

        public PoliticalPartyService(IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticalPartyDto> validator, ILoggerAdapter<PoliticalPartyService> logger)
        {
            _politicalPartyRepository = politicalPartyRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(PoliticalPartyDto politicalPartyDto)
        {
            _logger.LogDebug("Creating political party");

            _validator.ValidateAndThrow(politicalPartyDto);

            var partyExists = await _politicalPartyRepository.ExistsByNameAsync(politicalPartyDto.Name);

            if (partyExists)
            {
                var msg = $"Political party with name {politicalPartyDto.Name} already exists";
                _logger.LogWarn(msg);

                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(politicalPartyDto.Name), msg));
            }

            politicalPartyDto.Id = Guid.NewGuid();
            politicalPartyDto.Politicians.ForEach(x => x.Id = Guid.NewGuid());

            var result = await _politicalPartyRepository.CreateAsync(politicalPartyDto.ToPoliticalParty());

            if (result == true)
            {
                _logger.LogInfo("Political party with id {id} created", politicalPartyDto.Id);
            }
            else {
                _logger.LogError(null, "Unable to create political party");
            }

            return result;
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

            if (politicalParty is null) {
                _logger.LogWarn("Political party with id {id} not found", id.ToString());
            }

            return politicalParty?.ToPoliticalPartyDto();
        }
    }
}
