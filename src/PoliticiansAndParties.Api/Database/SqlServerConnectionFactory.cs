using System.Data;
using Microsoft.Data.SqlClient;

namespace PoliticiansAndParties.Api.Database;

public struct ConnectionStrings
{
    public ConnectionStrings(string masterConnection, string sqlConnection)
    {
        MasterConnection = masterConnection;
        SqlConnection = sqlConnection;
    }

    public string MasterConnection { get; }
    public string SqlConnection { get; }
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly ConnectionStrings _connectionStrings;

    public SqlServerConnectionFactory(ConnectionStrings connectionStrings)
    {
        _connectionStrings = connectionStrings;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionStrings.SqlConnection);
        await connection.OpenAsync();

        return connection;
    }

    public async Task<IDbConnection> CreateMasterConnectionAsync()
    {
        var connection = new SqlConnection(_connectionStrings.MasterConnection);
        await connection.OpenAsync();

        return connection;
    }
}
