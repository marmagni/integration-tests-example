using ThrowawayDb;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TestServer.Blocks.TestServices
{
    internal class SqlSandboxConnection : IDisposable
    {
        private string _sqlConnection;
        public SqlConnection? Connection;
        private ThrowawayDatabase? _database;

        public SqlSandboxConnection(IConfiguration configuration)
        {
            var sqlconn = configuration.GetConnectionString("SqlConn");
            var connBuilder = new SqlConnectionStringBuilder(sqlconn);

            connBuilder.InitialCatalog = "";
            _sqlConnection = connBuilder.ConnectionString;
        }

        public void OpenConnection()
        {
            Connection = _database.OpenConnection();
        }

        public async Task OpenConnectionAsync()
        {
            Connection = await _database.OpenConnectionAsync();
        }

        public async Task CreateDatabaseAsync()
        {
            _database = ThrowawayDatabase.Create(_sqlConnection);
            Connection = await _database.OpenConnectionAsync();
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }
}
