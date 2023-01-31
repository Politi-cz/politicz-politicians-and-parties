using Dapper;
using FluentMigrator.Model;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;
using System.Data;
using Z.Dapper.Plus;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticalPartyRepository : IPoliticalPartyRepository
    {
        readonly IDbConnectionFactory _connectionFactory;
        readonly IPoliticianRepository _politicianRepository;

        public PoliticalPartyRepository(IDbConnectionFactory connectionFactory, IPoliticianRepository politicianRepository)
        {
            _connectionFactory = connectionFactory;
            _politicianRepository= politicianRepository;
        }

        public async Task<bool> CreateAsync(PoliticalParty politicalParty)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                politicalParty.Id = await transaction.Connection.QuerySingleAsync<int>("INSERT INTO PoliticalParties (FrontEndId, Name, ImageUrl) OUTPUT INSERTED.Id VALUES(@FrontEndId, @Name, @ImageUrl)", politicalParty, transaction: transaction);
                politicalParty.Politicians.ForEach(x => x.PoliticalPartyId= politicalParty.Id);

                await _politicianRepository.CreateAllAsync(politicalParty.Politicians, transaction);

                var tagIds = await CreateTags(politicalParty.Tags, transaction);

                await CreatePoliticalParty_TagsRecords(tagIds, politicalParty.Id, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                // TODO add proper logging
                Console.WriteLine(e);
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

            var politicalParties = await connection.QueryAsync<PoliticalParty, string, PoliticalParty>(sql, (party, tagname) =>
            {
                party.Tags.Add(tagname);
                return party;
            }, splitOn: "Name");

            var result = politicalParties.GroupBy(x => x.Id).Select(g =>
            {
                var groupedParty = g.First();
                groupedParty.Tags = g.Select(p => p.Tags.Single()).ToList();
                return groupedParty;
            });

            if (result is null) { 
                return Enumerable.Empty<PoliticalParty>();
            }

            return result;
        }

        public async Task<PoliticalParty?> GetAsync(Guid frontEndId)
        {
            // TODO IT is only to get 1 political party, so instead of query use queryfirst
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = @"SELECT pp.Id, pp.FrontEndId, [Name], [ImageUrl], p.Id, p.FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId
                        FROM PoliticalParties pp LEFT JOIN Politicians p ON pp.Id = p.PoliticalPartyId
                        WHERE pp.FrontEndId = @FrontEndId ";

            // I know that where will be at most 1 political party thanks to WHERE clause so I can collect politicians on the way and skip the grouping part of result that gets rid of duplicated
            var politicians = new List<Politician>();
            var politicalParties = await connection.QueryAsync<PoliticalParty, Politician, PoliticalParty>(sql, (politicalParty, politician) =>
            {
                if (politician is not null) { 
                    politicians.Add(politician);
                }

                return politicalParty;
            },param: new { FrontEndId = frontEndId }, splitOn: "Id");

            if (politicalParties.Any()) {
                var politicalParty = politicalParties.First();
                politicalParty.Politicians = politicians;
                politicalParty.Tags = await LoadTags(politicalParty.Id, connection);

                return politicalParty; 
            }

            return null;

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

            var result = await connection.ExecuteScalarAsync<int>(sql ,param: new { Name = partyName });

            return result > 0;
        }

        private static async Task<List<string>> LoadTags(int politicalPartyId, IDbConnection connection) {
            var sql = "SELECT t.[Name] FROM Tags t INNER JOIN PoliticalParties_Tags pt ON pt.TagId = t.Id WHERE pt.PoliticalPartyId = @PoliticalPartyId";

            var tags = await connection.QueryAsync<string>(sql, param: new { PoliticalPartyId = politicalPartyId});

            if (tags is null) { 
                return new List<string>();
            }

            return tags.ToList();
        }

        private static async Task<IEnumerable<int>> CreateTags(IEnumerable<string> tags, IDbTransaction transaction) {
            var tagIdList = new List<int>();

            foreach (var tag in tags) {
                var tagId = await transaction.Connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Tags WHERE Name = @Name", new { Name = tag }, transaction: transaction);

                if (tagId is null)
                {
                    tagId = await transaction.Connection.QuerySingleAsync<int>("INSERT INTO Tags (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { Name = tag }, transaction: transaction);
                }

                tagIdList.Add((int)tagId);
            }

            return tagIdList;
        }

        private static async Task CreatePoliticalParty_TagsRecords(IEnumerable<int> tagIds, int politicalPartyId, IDbTransaction transaction) {
            foreach (var tagId in tagIds) {
                await transaction.Connection.ExecuteAsync("INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)", new { PoliticalPartyId = politicalPartyId, TagId = tagId }, transaction: transaction);
            }
        }

        
    }
}
