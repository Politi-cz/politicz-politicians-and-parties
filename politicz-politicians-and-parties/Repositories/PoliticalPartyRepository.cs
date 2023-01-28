using Dapper;
using FluentMigrator.Model;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;
using System.Data;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticalPartyRepository : IPoliticalPartyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PoliticalPartyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreatePoliticalParty(PoliticalParty politicalParty)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            // TODO Probably run in transaction, many operations done. Encapsulate in trycatch, if exception thrown abort transaction and rollback
            try
            {
                politicalParty.Id = await connection.QuerySingleAsync<int>("INSERT INTO PoliticalParties (FrontEndId, Name, ImageUrl) OUTPUT INSERTED.Id VALUES(@FrontEndId, @Name, @ImageUrl)", politicalParty);

                var tagIds = await CreateTags(politicalParty.Tags, connection);

                await CreatePoliticalParty_TagsRecords(tagIds, politicalParty.Id, connection);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<PoliticalParty>> GetPoliticalPartiesAsync()
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

        public async Task<PoliticalParty?> GetPoliticalPartyAsync(Guid frontEndId)
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
        public async Task<int?> GetPoliticalPartyInternalIdAsync(Guid frontEndId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = "SELECT Id FROM PoliticalParties WHERE FrontEndId = @FrontEndId";

            return await connection.QuerySingleOrDefaultAsync<int?>(sql, param: new { FrontEndId = frontEndId });
        }

        private async Task<List<string>> LoadTags(int politicalPartyId, IDbConnection connection) {
            var sql = "SELECT t.[Name] FROM Tags t INNER JOIN PoliticalParties_Tags pt ON pt.TagId = t.Id WHERE pt.PoliticalPartyId = @PoliticalPartyId";

            var tags = await connection.QueryAsync<string>(sql, param: new { PoliticalPartyId = politicalPartyId});

            if (tags is null) { 
                return new List<string>();
            }

            return tags.ToList();
        }

        private async Task<IEnumerable<int>> CreateTags(IEnumerable<string> tags, IDbConnection connection) {
            var tagIdList = new List<int>();

            foreach (var tag in tags) {
                var tagId = await connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Tags WHERE Name = @Name", new { Name = tag });

                if (tagId is null)
                {
                    tagId = await connection.QuerySingleAsync<int>("INSERT INTO Tags (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { Name = tag });
                }

                tagIdList.Add((int)tagId);
            }

            return tagIdList;
        }

        private async Task CreatePoliticalParty_TagsRecords(IEnumerable<int> tagIds, int politicalPartyId, IDbConnection connection) {
            foreach (var tagId in tagIds) {
                await connection.ExecuteAsync("INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)", new { PoliticalPartyId = politicalPartyId, TagId = tagId });
            }
        }

    }
}
