using Dapper;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;
using System.Data;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticianRepository : IPoliticianRepository
    {
        readonly IDbConnectionFactory _connectionFactory;

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

        // TODO: Pass IdbConnection as parametr, pass another parameter for transaction as IDbTransaction? and set default value to null so it can be used without specifying the transaction
        public async Task<bool> CreateAllAsync(IEnumerable<Politician> politicians, IDbTransaction transaction)
        {
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

        public async Task<bool> UpdateAsync(Politician politician)
        {
            var sql = @"UPDATE Politicians 
                        SET BirthDate = @BirthDate, FullName = @FullName, InstagramUrl = @InstagramUrl, TwitterUrl = @TwitterUrl, FacebookUrl = @FacebookUrl
                        WHERE FrontEndId = @FrontEndId";

            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync(sql, param: politician);

            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid frontEndId)
        {
            var sql = @"DELETE FROM Politicians 
                        WHERE FrontEndId = @FrontEndId";

            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync(sql, param: new { FrontEndId = frontEndId });

            return result > 0;
        }
    }
}
