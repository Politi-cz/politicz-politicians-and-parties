using Microsoft.EntityFrameworkCore;
using politicz_politicians_and_parties.Data;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticianRepository : IPoliticianRepository
    {
        private readonly ApplicationDbContext _db;

        public PoliticianRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Politician?> GetPolitician(Guid id)
        {
            return await _db.Politicians.Where(x => x.FrontEndId.Equals(id)).FirstOrDefaultAsync();
        }
    }
}
