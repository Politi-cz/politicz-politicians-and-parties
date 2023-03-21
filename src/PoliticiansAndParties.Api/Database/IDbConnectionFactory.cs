namespace PoliticiansAndParties.Api.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnection();

    Task<IDbConnection> CreateMasterConnection();
}
