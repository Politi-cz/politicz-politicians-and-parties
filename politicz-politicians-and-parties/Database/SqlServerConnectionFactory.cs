using Microsoft.Data.SqlClient;
using System.Data;

namespace politicz_politicians_and_parties.Database
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlServerConnectionFactory(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            return connection;
        }

        public async Task<IDbConnection> CreateMasterConnectionAsync()
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("MasterConnection"));
            await connection.OpenAsync();

            return connection;
        }
    }
}
