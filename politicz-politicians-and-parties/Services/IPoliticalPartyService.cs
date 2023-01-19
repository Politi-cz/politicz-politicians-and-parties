using politicz_politicians_and_parties.Dtos;

namespace politicz_politicians_and_parties.Services
{
    public interface IPoliticalPartyService
    {
        Task<PoliticalPartyDto?> GetPoliticalPartyAsync(Guid id);
        Task<IEnumerable<PoliticalPartySideNavDto>> GetPoliticalPartiesAsync();
    }
}
