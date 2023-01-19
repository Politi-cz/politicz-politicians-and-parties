using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public interface IPoliticianRepository
    {
        Task<Politician?> GetPoliticianAsync(Guid frontEndId);
    }
}
