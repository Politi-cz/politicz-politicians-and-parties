namespace PoliticiansAndParties.Api.Database;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task InitializeAsync(string dbName)
    {
        string query = "SELECT * FROM sys.databases WHERE name = @name";
        var parameters = new DynamicParameters();
        parameters.Add("name", dbName);

        using var connection = await _connectionFactory.CreateMasterConnectionAsync();
        var records = connection.Query(query, parameters);
        if (!records.Any())
        {
            _ = await connection.ExecuteAsync($"CREATE DATABASE [{dbName}]");
        }
    }
}
