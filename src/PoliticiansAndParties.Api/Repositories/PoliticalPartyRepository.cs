﻿namespace PoliticiansAndParties.Api.Repositories;

public class PoliticalPartyRepository : IPoliticalPartyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggerAdapter<PoliticalPartyRepository> _logger;
    private readonly IPoliticianRepository _politicianRepository;

    public PoliticalPartyRepository(IDbConnectionFactory connectionFactory, IPoliticianRepository politicianRepository, ILoggerAdapter<PoliticalPartyRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _politicianRepository = politicianRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PoliticalParty>> GetAll()
    {
        using var connection = await _connectionFactory.CreateConnection();

        const string sql = @"SELECT pp.Id, pp.FrontEndId, pp.[Name], pp.[ImageUrl], t.[Name] FROM PoliticalParties pp
                        INNER JOIN PoliticalParties_Tags pt ON pp.Id = pt.PoliticalPartyId
                        INNER JOIN Tags t ON pt.TagId = t.Id";

        var politicalPartyDictionary = new Dictionary<int, PoliticalParty>();

        var politicalParties = connection.Query<PoliticalParty, string, PoliticalParty>(
            sql,
            (party, tagname) =>
            {
                if (!politicalPartyDictionary.TryGetValue(party.Id, out var politicalPartyEntry))
                {
                    politicalPartyEntry = party;
                    politicalPartyEntry.Tags = new HashSet<string>();
                    politicalPartyDictionary.Add(politicalPartyEntry.Id, politicalPartyEntry);
                }

                _ = politicalPartyEntry.Tags.Add(tagname);

                return politicalPartyEntry;
            },
            splitOn: "Name").Distinct().ToList();

        return politicalParties;
    }

    public async Task<ResultOrNotFound<PoliticalParty>> GetOne(Guid frontEndId)
    {
        using var connection = await _connectionFactory.CreateConnection();

        const string sql =
            @"SELECT pp.Id, pp.FrontEndId, [Name], pp.ImageUrl, p.Id, p.FrontEndId, BirthDate, FullName,p.ImageUrl, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId
                        FROM PoliticalParties pp INNER JOIN Politicians p ON pp.Id = p.PoliticalPartyId
                        WHERE pp.FrontEndId = @FrontEndId ";

        var politicalPartyDictionary = new Dictionary<int, PoliticalParty>();

        // TODO maybe write a generic helper method for M:N dapper Relation
        var politicalParties = connection.Query<PoliticalParty, Politician, PoliticalParty>(
            sql,
            (politicalParty, politician) =>
            {
                if (!politicalPartyDictionary.TryGetValue(politicalParty.Id, out var politicalPartyEntry))
                {
                    politicalPartyEntry = politicalParty;
                    politicalPartyEntry.Politicians = new List<Politician>();
                    politicalPartyDictionary.Add(politicalPartyEntry.Id, politicalPartyEntry);
                }

                politicalPartyEntry.Politicians.Add(politician);

                return politicalPartyEntry;
            },
            new { FrontEndId = frontEndId },
            splitOn: "Id").Distinct().ToList();

        // FrontEndId is specified in WHERE clausule so at most 1 result should exist if there is more than one throw exception
        var politicalParty = politicalParties.SingleOrDefault();

        if (politicalParty is null)
        {
            return default(NotFound);
        }

        politicalParty.Tags = await LoadTags(politicalParty.Id, connection);

        return politicalParty;
    }

    public async Task<ResultOrNotFound<int>> GetInternalId(Guid frontEndId)
    {
        using var connection = await _connectionFactory.CreateConnection();

        const string sql = "SELECT Id FROM PoliticalParties WHERE FrontEndId = @FrontEndId";

        int id = await connection.QuerySingleOrDefaultAsync<int>(sql, new { FrontEndId = frontEndId });

        return id == 0 ? default(NotFound) : id;
    }

    public async Task<bool> ExistsByName(string partyName)
    {
        using var connection = await _connectionFactory.CreateConnection();

        const string sql = "SELECT COUNT(1) FROM PoliticalParties WHERE Name = @Name";

        int result = await connection.ExecuteScalarAsync<int>(sql, new { Name = partyName });

        return result > 0;
    }

    public async Task<bool> ExistsByName(string partyName, Guid frontEndId)
    {
        const string sql = "SELECT COUNT(1) FROM PoliticalParties WHERE Name = @Name AND FrontEndId != @FrontEndId";

        using var connection = await _connectionFactory.CreateConnection();
        int result = await connection.ExecuteScalarAsync<int>(sql, new { Name = partyName, FrontEndId = frontEndId });

        return result > 0;
    }

    public async Task<ResultOrNotFound<PoliticalParty>> Update(PoliticalParty politicalParty)
    {
        const string sql = @"UPDATE PoliticalParties SET Name = @Name, ImageUrl = @ImageUrl
                             OUTPUT INSERTED.*
                             WHERE FrontEndId = @FrontEndId";

        using var connection = await _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            var updatedParty = await transaction.Connection.QuerySingleOrDefaultAsync<PoliticalParty>(sql, politicalParty, transaction);

            if (updatedParty is null)
            {
                _logger.LogWarn("Political party with id {Id} not found", politicalParty.FrontEndId);
                transaction.Rollback();

                return default(NotFound);
            }

            updatedParty.Tags = politicalParty.Tags;
            var tagIds = await CreateTagsRecords(updatedParty, transaction);

            await DeleteDanglingTags(updatedParty.Id, tagIds, transaction);
            transaction.Commit();

            return updatedParty;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "Unable updating party with id {Id}", politicalParty.Id);
            transaction.Rollback();

            throw;
        }
    }

    public async Task<SuccessOrNotFound> Delete(Guid partyId)
    {
        using var connection = await _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sqlDeleteParty = "DELETE FROM PoliticalParties WHERE FrontEndId = @PartyId";
            const string sqlDeleteTagsRecords = @"DELETE pt FROM PoliticalParties_Tags pt
                                            INNER JOIN PoliticalParties pp ON pt.PoliticalPartyId = pp.Id
                                            WHERE pp.FrontEndId = @PartyId";

            _ = await transaction.Connection.ExecuteAsync(sqlDeleteTagsRecords, new { PartyId = partyId }, transaction);

            int result =
                await transaction.Connection.ExecuteAsync(sqlDeleteParty, new { PartyId = partyId }, transaction);

            if (result == 0)
            {
                _logger.LogWarn("Political party with id {Id} not found", partyId);
                transaction.Rollback();

                return default(NotFound);
            }

            transaction.Commit();
            return default(Success);
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "Unable to delete party with id {Id}", partyId);
            transaction.Rollback();
            throw;
        }
    }

    public async Task<PoliticalParty> Create(PoliticalParty politicalParty)
    {
        using var connection = await _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            politicalParty.Id = await transaction.Connection.QuerySingleAsync<int>(
                "INSERT INTO PoliticalParties ([FrontEndId], [Name], [ImageUrl]) OUTPUT INSERTED.* VALUES(@FrontEndId, @Name, @ImageUrl)",
                politicalParty,
                transaction);

            _ = await CreateTagsRecords(politicalParty, transaction);

            politicalParty.Politicians.ForEach(x => x.PoliticalPartyId = politicalParty.Id);

            _ = await _politicianRepository.CreateAll(politicalParty.Politicians, transaction);

            transaction.Commit();
            return politicalParty;
        }
        catch (SqlException e)
        {
            _logger.LogError(e, "Political party creation failed");
            transaction.Rollback();
            throw;
        }
    }

    private static async Task<HashSet<string>> LoadTags(int politicalPartyId, IDbConnection connection)
    {
        const string sql =
            "SELECT t.[Name] FROM Tags t INNER JOIN PoliticalParties_Tags pt ON pt.TagId = t.Id WHERE pt.PoliticalPartyId = @PoliticalPartyId";

        var tags = await connection.QueryAsync<string>(sql, new { PoliticalPartyId = politicalPartyId });

        return tags is null ? new HashSet<string>() : tags.ToHashSet();
    }

    private async Task DeleteDanglingTags(int partyId, IEnumerable<int> currentTags, IDbTransaction transaction)
    {
        const string sql = "DELETE FROM PoliticalParties_Tags WHERE PoliticalPartyId = @PartyId AND TagId NOT IN @Tags";
        int result =
            await transaction.Connection.ExecuteAsync(sql, new { PartyId = partyId, Tags = currentTags }, transaction);
        _logger.LogDebug("Deleted {result} politicalParties_tags records", result);
    }

    private async Task<IEnumerable<int>> CreateTagsRecords(PoliticalParty politicalParty, IDbTransaction transaction)
    {
        var tagIds = new List<int>();

        if (politicalParty.Tags.Count > 0)
        {
            foreach (string tag in politicalParty.Tags)
            {
                int? tagId = await GetTagId(tag, transaction);

                tagId ??= await CreateTag(tag, transaction);

                bool partyTagRecordExists =
                    await ExistsPoliticalParties_TagsRecord(politicalParty.Id, (int)tagId, transaction);

                if (!partyTagRecordExists)
                {
                    _ = await transaction.Connection.ExecuteAsync(
                        "INSERT INTO PoliticalParties_Tags (PoliticalPartyId, TagId) VALUES (@PoliticalPartyId, @TagId)",
                        new { PoliticalPartyId = politicalParty.Id, TagId = tagId },
                        transaction);
                }

                tagIds.Add((int)tagId);
            }
        }

        return tagIds;
    }

    private async Task<int?> GetTagId(string tag, IDbTransaction transaction) => await transaction.Connection.QuerySingleOrDefaultAsync<int?>(
            "SELECT Id FROM Tags WHERE Name = @Name",
            new { Name = tag },
            transaction);

    private async Task<int> CreateTag(string tag, IDbTransaction transaction)
    {
        int tagId = await transaction.Connection.QuerySingleAsync<int>(
            "INSERT INTO Tags (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { Name = tag }, transaction);
        _logger.LogDebug("Created tag with id {Id}", tagId);

        return tagId;
    }

    private async Task<bool> ExistsPoliticalParties_TagsRecord(int politicalPartyId, int tagId, IDbTransaction transaction)
    {
        string sql = "SELECT COUNT(1) FROM PoliticalParties_Tags WHERE PoliticalPartyId = @PartyId AND TagId = @TagId";
        int result = await transaction.Connection.ExecuteScalarAsync<int>(
            sql,
            new { PartyId = politicalPartyId, TagId = tagId },
            transaction);

        return result > 0;
    }
}
