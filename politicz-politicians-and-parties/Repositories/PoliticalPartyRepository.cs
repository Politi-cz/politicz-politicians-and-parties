using Dapper;
using Microsoft.Data.SqlClient;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Models;
using System.Data;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticalPartyRepository : IPoliticalPartyRepository
    {
        readonly IDbConnectionFactory _connectionFactory;
        readonly IPoliticianRepository _politicianRepository;
        readonly ILoggerAdapter<PoliticalPartyRepository> _logger;

        public PoliticalPartyRepository(IDbConnectionFactory connectionFactory, IPoliticianRepository politicianRepository, ILoggerAdapter<PoliticalPartyRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _politicianRepository = politicianRepository;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(PoliticalParty politicalParty)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                politicalParty.Id = await transaction.Connection.QuerySingleAsync<int>("INSERT INTO PoliticalParties (FrontEndId, Name, ImageUrl) OUTPUT INSERTED.Id VALUES(@FrontEndId, @Name, @ImageUrl)", politicalParty, transaction: transaction);

                await CreateTags(politicalParty, transaction);

                politicalParty.Politicians.ForEach(x => x.PoliticalPartyId = politicalParty.Id);

                await _politicianRepository.CreateAllAsync(politicalParty.Politicians, transaction);

                transaction.Commit();
                return true;
            }
            catch (SqlException e)
            {
                _logger.LogError(e, "Political party creation failed");
                transaction.Rollback();
                return false;
            }
        }

        public async Task<IEnumerable<PoliticalParty>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = (@"SELECT pp.Id, pp.FrontEndId, pp.[Name], pp.[ImageUrl], t.[Name] FROM PoliticalParties pp 
                        INNER JOIN PoliticalParties_Tags pt ON pp.Id = pt.PoliticalPartyId
                        INNER JOIN Tags t ON pt.TagId = t.Id");

            var politicalPartyDictionary = new Dictionary<int, PoliticalParty>();

            var politicalParties = connection.Query<PoliticalParty, string, PoliticalParty>(sql, (party, tagname) =>
            {
                PoliticalParty? politicalPartyEntry;

                if (!politicalPartyDictionary.TryGetValue(party.Id, out politicalPartyEntry))
                {
                    politicalPartyEntry = party;
                    politicalPartyEntry.Tags = new HashSet<string>();
                    politicalPartyDictionary.Add(politicalPartyEntry.Id, politicalPartyEntry);

                }

                politicalPartyEntry.Tags.Add(tagname);


                return politicalPartyEntry;
            }, splitOn: "Name").Distinct().ToList();

            return politicalParties;
        }

        public async Task<PoliticalParty?> GetAsync(Guid frontEndId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = @"SELECT pp.Id, pp.FrontEndId, [Name], [ImageUrl], p.Id, p.FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId
                        FROM PoliticalParties pp INNER JOIN Politicians p ON pp.Id = p.PoliticalPartyId
                        WHERE pp.FrontEndId = @FrontEndId ";

            var politicalPartyDictionary = new Dictionary<int, PoliticalParty>();

            // TODO maybe write a generic helper method for M:N dapper Relation
            var politicalParties = connection.Query<PoliticalParty, Politician, PoliticalParty>(sql, (politicalParty, politician) =>
            {
                PoliticalParty? politicalPartyEntry;

                if (!politicalPartyDictionary.TryGetValue(politicalParty.Id, out politicalPartyEntry))
                {
                    politicalPartyEntry = politicalParty;
                    politicalPartyEntry.Politicians = new List<Politician>();
                    politicalPartyDictionary.Add(politicalPartyEntry.Id, politicalPartyEntry);

                }

                politicalPartyEntry.Politicians.Add(politician);

                return politicalPartyEntry;
            }, param: new { FrontEndId = frontEndId }, splitOn: "Id").Distinct().ToList();

            // FrontEndId is specified in WHERE clausule so at most 1 result should exist if there is more than one throw exception
            var politicalParty = politicalParties.SingleOrDefault();

            if (politicalParty is not null)
            {
                politicalParty.Tags = await LoadTags(politicalParty.Id, connection);
            }

            return politicalParty;
        }
        public async Task<int?> GetInternalIdAsync(Guid frontEndId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = "SELECT Id FROM PoliticalParties WHERE FrontEndId = @FrontEndId";

            return await connection.QuerySingleOrDefaultAsync<int?>(sql, param: new { FrontEndId = frontEndId });
        }

        public async Task<bool> ExistsByNameAsync(string partyName)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = "SELECT COUNT(1) FROM PoliticalParties WHERE Name = @Name";

            var result = await connection.ExecuteScalarAsync<int>(sql, param: new { Name = partyName });

            return result > 0;
        }

        public async Task<bool> UpdateAsync(PoliticalParty politicalParty)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {

                var updatedPartyId = await transaction.Connection.QuerySingleAsync<int>("UPDATE PoliticalParties SET Name = @Name, ImageUrl = @ImageUrl OUTPUT INSERTED.Id WHERE FrontEndId = @FrontEndId", param: politicalParty, transaction: transaction);

                if (updatedPartyId == 0)
                {
                    _logger.LogWarn("Political party with id {id} not found", politicalParty.FrontEndId);

                    transaction.Commit();
                    return false;
                }

                politicalParty.Id = updatedPartyId;

                // TODO: Exception thrown when an existing tag already assigned to a party is passed to update
                var tagIds = await CreateTags(politicalParty, transaction);

                await DeleteDanglingTags(politicalParty.Id, tagIds, transaction);
                transaction.Commit();

                return true;
            }
            catch (SqlException e)
            {
                _logger.LogError(e, "Unable updating party with id {id}", politicalParty.Id);
                transaction.Rollback();

                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid partyId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sqlDeleteParty = "DELETE FROM PoliticalParties WHERE FrontEndId = @PartyId";
                var sqlDeleteTagsRecords = @"DELETE pt FROM PoliticalParties_Tags pt
                                            INNER JOIN PoliticalParties pp ON pt.PoliticalPartyId = pp.Id
                                            WHERE pp.FrontEndId = @PartyId";

                await transaction.Connection.ExecuteAsync(sqlDeleteTagsRecords, param: new { PartyId = partyId }, transaction: transaction);

                var result = await transaction.Connection.ExecuteAsync(sqlDeleteParty, param: new { PartyId = partyId }, transaction: transaction);

                if (result == 0)
                {
                    _logger.LogWarn("Political party with id {id} not found", partyId);
                    transaction.Rollback();

                    return false;
                }

                transaction.Commit();
                return result > 0;
            }
            catch (SqlException e)
            {
                _logger.LogError(e, "Unable to delete party with id {id}", partyId);
                transaction.Rollback();
                throw;
            }
        }

        private async Task DeleteDanglingTags(int partyId, IEnumerable<int> currentTags, IDbTransaction transaction)
        {
            var sql = "DELETE FROM PoliticalParties_Tags WHERE PoliticalPartyId = @PartyId AND TagId NOT IN @Tags";
            var result = await transaction.Connection.ExecuteAsync(sql, param: new { PartyId = partyId, Tags = currentTags }, transaction: transaction);
            _logger.LogDebug("Deleted {result} politicalParties_tags records", result);
        }

        private async Task<IEnumerable<int>> CreateTags(PoliticalParty politicalParty, IDbTransaction transaction)
        {
            var tagIds = new List<int>();

            if (politicalParty.Tags is not null && politicalParty.Tags.Count > 0)
            {
                foreach (var tag in politicalParty.Tags)
                {
                    var tagId = await CreateOrExistTag(tag, transaction);

                    await transaction.Connection.ExecuteAsync("INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)",
                              new { PoliticalPartyId = politicalParty.Id, TagId = tagId }, transaction: transaction);

                    tagIds.Add(tagId);
                }
            }

            return tagIds;
        }

        private static async Task<HashSet<string>> LoadTags(int politicalPartyId, IDbConnection connection)
        {
            var sql = "SELECT t.[Name] FROM Tags t INNER JOIN PoliticalParties_Tags pt ON pt.TagId = t.Id WHERE pt.PoliticalPartyId = @PoliticalPartyId";

            var tags = await connection.QueryAsync<string>(sql, param: new { PoliticalPartyId = politicalPartyId });

            if (tags is null)
            {
                return new HashSet<string>();
            }

            return tags.ToHashSet();
        }

        // TODO: Separate into 2 methods for getting tag and creating tag.
        private async Task<int> CreateOrExistTag(string tag, IDbTransaction transaction)
        {
            var tagId = await transaction.Connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Tags WHERE Name = @Name", new { Name = tag }, transaction: transaction);

            if (tagId is null)
            {
                tagId = await transaction.Connection.QuerySingleAsync<int>("INSERT INTO Tags (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { Name = tag }, transaction: transaction);
                _logger.LogDebug("Created tag with id {id}", tagId);
            }

            return (int)tagId;
        }


    }
}
