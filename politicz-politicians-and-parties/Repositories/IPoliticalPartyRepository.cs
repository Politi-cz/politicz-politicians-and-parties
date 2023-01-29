using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public interface IPoliticalPartyRepository
    {
        Task<IEnumerable<PoliticalParty>> GetAllAsync();
        Task<PoliticalParty?> GetAsync(Guid frontEndId);
        Task<int?> GetInternalIdAsync(Guid frontEndId);
        Task<bool> CreateAsync(PoliticalParty politicalParty);
        Task<bool> ExistsByNameAsync(string partyName);
    }
}
