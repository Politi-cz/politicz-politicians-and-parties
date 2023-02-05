using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Validators;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticalPartyService : IPoliticalPartyService
    {
        readonly IPoliticalPartyRepository _politicalPartyRepository;
        readonly IValidator<PoliticalPartyDto> _validator;

        public PoliticalPartyService(IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticalPartyDto> validator)
        {
            _politicalPartyRepository = politicalPartyRepository;
            _validator = validator;
        }

        public async Task<bool> CreateAsync(PoliticalPartyDto politicalPartyDto)
        {
            _validator.ValidateAndThrow(politicalPartyDto);

            var partyExists = await _politicalPartyRepository.ExistsByNameAsync(politicalPartyDto.Name);

            if (partyExists)
            {
                var msg = $"Political party with name {politicalPartyDto.Name} already exists";
                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(politicalPartyDto.Name), msg));
            }

            politicalPartyDto.Id = Guid.NewGuid();
            politicalPartyDto.Politicians.ForEach(x => x.Id = Guid.NewGuid());

            var politicalParty = politicalPartyDto.ToPoliticalParty();

            return await _politicalPartyRepository.CreateAsync(politicalParty);
        }

        public async Task<IEnumerable<PoliticalPartySideNavDto>> GetAllAsync()
        {
            var politicalParties = await _politicalPartyRepository.GetAllAsync();
            var politicalPartiesSideNav = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

            return politicalPartiesSideNav;
        }

        public async Task<PoliticalPartyDto?> GetOneAsync(Guid id)
        {
            var politicalParty = await _politicalPartyRepository.GetAsync(id);

            return politicalParty?.ToPoliticalPartyDto();
        }
    }
}
