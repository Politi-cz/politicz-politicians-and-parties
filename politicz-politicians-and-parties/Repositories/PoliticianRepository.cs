using Dapper;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticianRepository : IPoliticianRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PoliticianRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreatePoliticianAsync(Politician politician)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"INSERT INTO Politicians (FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId) 
                        VALUES (@FrontEndId, @BirthDate, @FullName, @InstagramUrl, @TwitterUrl, @FacebookUrl, @PoliticalPartyId)";

            var result = await connection.ExecuteAsync(sql, politician);

            return result > 0;
        }

        public async Task<Politician?> GetPoliticianAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QuerySingleOrDefaultAsync<Politician>("SELECT * FROM Politicians WHERE FrontEndId = @FrontEndId", new { FrontEndId = id });
        }
    }
}
