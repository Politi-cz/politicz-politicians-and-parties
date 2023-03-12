namespace PoliticiansAndParties.Api.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();

    Task<IDbConnection> CreateMasterConnectionAsync();
}
