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

        public async Task<bool> CreatePoliticalParty(PoliticalPartyCreateDto politicalPartyDto)
        {
            // Add fluent validator validation, figure out how to return validation errors to user

            // Add database fetch to chech if political party id or name exists, update migration so the political party name is unique
            politicalPartyDto.Id = Guid.NewGuid();
            var politicalParty = politicalPartyDto.ToPoliticalParty();
        
            var created = await _politicalPartyRepository.CreatePoliticalParty(politicalParty);
            if (!created) { 
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<PoliticalPartySideNavDto>> GetPoliticalPartiesAsync()
        {
            var politicalParties = await _politicalPartyRepository.GetPoliticalPartiesAsync();
            var politicalPartiesSideNav = politicalParties.Select(x => x.ToPoliticalPartySideNavDto());

            return politicalPartiesSideNav;
        }

        public async Task<PoliticalPartyDto?> GetPoliticalPartyAsync(Guid id)
        {
            var politicalParty = await _politicalPartyRepository.GetPoliticalPartyAsync(id);

            return politicalParty?.ToPoliticalPartyDto();
        }
    }
}
