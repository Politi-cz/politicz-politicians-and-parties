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

                if (politicalParty.Tags is not null && politicalParty.Tags.Count > 0)
                {
                    foreach (var tag in politicalParty.Tags)
                    {
                        var tagId = await CreateOrExistTag(tag, transaction);

                        await transaction.Connection.ExecuteAsync("INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)",
                                  new { PoliticalPartyId = politicalParty.Id, TagId = tagId }, transaction: transaction);
                    }

                }

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

        private static async Task<int> CreateOrExistTag(string tag, IDbTransaction transaction)
        {
            var tagId = await transaction.Connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Tags WHERE Name = @Name", new { Name = tag }, transaction: transaction);

            if (tagId is null)
            {
                tagId = await transaction.Connection.QuerySingleAsync<int>("INSERT INTO Tags (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { Name = tag }, transaction: transaction);
            }

            return (int)tagId;
        }

        private static async Task CreatePoliticalParty_TagsRecords(IEnumerable<int> tagIds, int politicalPartyId, IDbTransaction transaction)
        {
            foreach (var tagId in tagIds)
            {
                await transaction.Connection.ExecuteAsync("INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)", new { PoliticalPartyId = politicalPartyId, TagId = tagId }, transaction: transaction);
            }
        }


    }
}
