using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Repositories;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticalPartyService : IPoliticalPartyService
    {
        private readonly IPoliticalPartyRepository _politicalPartyRepository;

        public PoliticalPartyService(IPoliticalPartyRepository politicalPartyRepository)
        {
            _politicalPartyRepository = politicalPartyRepository;
        }

        public async Task<bool> CreateAsync(PoliticalPartyDto politicalPartyDto)
        {
            // Add fluent validator validation, figure out how to return validation errors to user

            var partyExists = await _politicalPartyRepository.ExistsByNameAsync(politicalPartyDto.Name);

            if (partyExists)
            {
                throw new ValidationException($"Political party with name {politicalPartyDto.Name} already exists");
            }

            var politicalParty = politicalPartyDto.ToPoliticalParty();
        
            var created = await _politicalPartyRepository.CreateAsync(politicalParty);
            if (!created) { 
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<PoliticalPartySideNavDto>> GetAllAsync()
        {
            var politicalParties = await _politicalPartyRepository.GetAllAsync();
            var politicalPartiesSideNav = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

            return politicalPartiesSideNav;
        }

        public async Task<PoliticalPartyDto?> GetAsync(Guid id)
        {
            var politicalParty = await _politicalPartyRepository.GetAsync(id);

            return politicalParty?.ToPoliticalPartyDto();
        }
    }
}
