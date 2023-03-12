namespace PoliticiansAndParties.Api.Repositories;

public interface IPoliticalPartyRepository
{
    Task<IEnumerable<PoliticalParty>> GetAllAsync();

    Task<PoliticalParty?> GetAsync(Guid frontEndId);

    Task<int?> GetInternalIdAsync(Guid frontEndId);

    Task<PoliticalParty> CreateAsync(PoliticalParty politicalParty);

    Task<bool> ExistsByNameAsync(string partyName);

    Task<bool> UpdateAsync(PoliticalParty politicalParty);

    Task<bool> DeleteAsync(Guid partyId);
}
