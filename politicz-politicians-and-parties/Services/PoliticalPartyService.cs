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
