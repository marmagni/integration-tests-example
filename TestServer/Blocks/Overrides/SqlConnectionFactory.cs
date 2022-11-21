using System.Data;
using ApiSample.Persistence.Dapper.Blocks;
using TestServer.Blocks.TestServices;

namespace TestServer.Blocks.Overrides
{
    internal class SqlConnectionFactory : IConnectionFactory
    {
        private SqlSandboxConnection _connection;

        public SqlConnectionFactory(SqlSandboxConnection connection)
        {
            _connection = connection;
        }

        public IDbConnection CreateConnection()
        {
            return _connection.Connection;
        }
    }    
}
