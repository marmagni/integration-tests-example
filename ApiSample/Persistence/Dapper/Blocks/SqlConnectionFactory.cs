using System.Data;
using Microsoft.Data.SqlClient;

namespace ApiSample.Persistence.Dapper.Blocks
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlConn");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
