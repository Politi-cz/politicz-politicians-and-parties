using System.Data;

namespace politicz_politicians_and_parties.Database
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
        Task<IDbConnection> CreateMasterConnectionAsync();
    }
}
