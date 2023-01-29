using Dapper;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;
using System.Data;
using Z.Dapper.Plus;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticianRepository : IPoliticianRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PoliticianRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreateOneAsync(Politician politician)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"INSERT INTO Politicians (FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId) 
                        VALUES (@FrontEndId, @BirthDate, @FullName, @InstagramUrl, @TwitterUrl, @FacebookUrl, @PoliticalPartyId)";

            var result = await connection.ExecuteAsync(sql, politician);

            return result > 0;
        }

        public async Task<bool> CreateAllAsync(IEnumerable<Politician> politicians, IDbTransaction transaction) {
            var sql = @"INSERT INTO Politicians (FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId)
                        VALUES (@FrontEndId, @BirthDate, @FullName, @InstagramUrl, @TwitterUrl, @FacebookUrl, @PoliticalPartyId)";

            var result = await transaction.Connection.ExecuteAsync(sql, politicians, transaction: transaction);

            return result > 0;
        }

        public async Task<Politician?> GetAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QuerySingleOrDefaultAsync<Politician>("SELECT * FROM Politicians WHERE FrontEndId = @FrontEndId", new { FrontEndId = id });
        }
    }
}
