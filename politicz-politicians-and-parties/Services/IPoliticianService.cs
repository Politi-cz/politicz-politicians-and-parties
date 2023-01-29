using politicz_politicians_and_parties.Dtos;

namespace politicz_politicians_and_parties.Services
{
    public interface IPoliticianService
    {
        Task<PoliticianDto?> GetAsync(Guid id);
        Task<bool> CreateAsync(Guid partyId, PoliticianDto politicianDto);
    }
}
