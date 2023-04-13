using Microsoft.Extensions.Options;

namespace PoliticiansAndParties.Api.Database;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _databaseOptions;

    public SqlServerConnectionFactory(IOptions<PoliticiansPartiesOptions> options) => _databaseOptions = options.Value.Database;

    public async Task<IDbConnection> CreateConnection()
    {
        var connection = new SqlConnection(_databaseOptions.DefaultConnection);
        await connection.OpenAsync();

        return connection;
    }

    public async Task<IDbConnection> CreateMasterConnection()
    {
        var connection = new SqlConnection(_databaseOptions.MasterConnection);
        await connection.OpenAsync();

        return connection;
    }
}
