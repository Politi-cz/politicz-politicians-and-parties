namespace PoliticiansAndParties.Api.Database;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    // TODO: Instead of struct implement IOptions pattern
    private readonly ConnectionStrings _connectionStrings;

    public SqlServerConnectionFactory(ConnectionStrings connectionStrings) => _connectionStrings = connectionStrings;

    public async Task<IDbConnection> CreateConnection()
    {
        var connection = new SqlConnection(_connectionStrings.SqlConnection);
        await connection.OpenAsync();

        return connection;
    }

    public async Task<IDbConnection> CreateMasterConnection()
    {
        var connection = new SqlConnection(_connectionStrings.MasterConnection);
        await connection.OpenAsync();

        return connection;
    }
}
