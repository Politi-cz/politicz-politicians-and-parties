using politicz_politicians_and_parties.Models;
using System.Data;

namespace politicz_politicians_and_parties.Repositories
{
    public interface IPoliticianRepository
    {
        Task<Politician?> GetAsync(Guid frontEndId);
        Task<bool> CreateOneAsync(Politician politician);
        Task<bool> CreateAllAsync(IEnumerable<Politician> politicians, IDbTransaction transaction);
    }
}
