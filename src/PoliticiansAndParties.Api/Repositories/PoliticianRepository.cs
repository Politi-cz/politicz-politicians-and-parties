namespace PoliticiansAndParties.Api.Repositories;

public class PoliticianRepository : IPoliticianRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PoliticianRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<Politician> CreateOne(Politician politician)
    {
        const string sql =
            @"INSERT INTO Politicians (FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId)
                        OUTPUT INSERTED.*
                        VALUES (@FrontEndId, @BirthDate, @FullName, @InstagramUrl, @TwitterUrl, @FacebookUrl, @PoliticalPartyId)";

        using var connection = await _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<Politician>(sql, politician);
    }

    // TODO: Pass IdbConnection as parametr, pass another parameter for transaction as IDbTransaction? and set default value to null so it can be used without specifying the transaction
    public async Task<bool> CreateAll(IEnumerable<Politician> politicians, IDbTransaction transaction)
    {
        const string sql =
            @"INSERT INTO Politicians (FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId)
                        VALUES (@FrontEndId, @BirthDate, @FullName, @InstagramUrl, @TwitterUrl, @FacebookUrl, @PoliticalPartyId)";

        int result = await transaction.Connection.ExecuteAsync(sql, politicians, transaction);

        return result > 0;
    }

    public async Task<ResultOrNotFound<Politician>> Get(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnection();

        var politician = await connection.QuerySingleOrDefaultAsync<Politician>(
            "SELECT * FROM Politicians WHERE FrontEndId = @FrontEndId", new { FrontEndId = id });

        return politician is null ? default(NotFound) : politician;
    }

    public async Task<ResultOrNotFound<Politician>> Update(Politician politician)
    {
        const string sql = @"UPDATE Politicians
                        SET BirthDate = @BirthDate, FullName = @FullName, InstagramUrl = @InstagramUrl, TwitterUrl = @TwitterUrl, FacebookUrl = @FacebookUrl
                        OUTPUT INSERTED.*
                        WHERE FrontEndId = @FrontEndId";

        using var connection = await _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<Politician>(sql, politician);

        return result is null ? default(NotFound) : result;
    }

    public async Task<SuccessOrNotFound> Delete(Guid frontEndId)
    {
        const string sql = @"DELETE FROM Politicians
                        WHERE FrontEndId = @FrontEndId";

        using var connection = await _connectionFactory.CreateConnection();
        int result = await connection.ExecuteAsync(sql, new { FrontEndId = frontEndId });

        return result > 0 ? default(Success) : default(NotFound);
    }
}
