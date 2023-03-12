using politicz_politicians_and_parties.Dtos;

namespace politicz_politicians_and_parties.Services
{
    public interface IPoliticalPartyService
    {
        Task<PoliticalPartyDto?> GetOneAsync(Guid id);
        Task<IEnumerable<PoliticalPartySideNavDto>> GetAllAsync();
        Task<bool> CreateAsync(PoliticalPartyDto politicalParty);

        Task<bool> UpdateAsync(UpdatePoliticalPartyDto updatePoliticalParty);
        Task<bool> DeleteAsync(Guid partyId);
    }
}
