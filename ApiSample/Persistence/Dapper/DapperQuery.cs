using ApiSample.Persistence.Dapper.Blocks;
using Dapper;

namespace ApiSample.Persistence.Dapper
{
    public class DapperQuery : IDapperQuery
    {
        private readonly IConnectionFactory _connectionFactory;

        public DapperQuery(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<T>> QueryAsync<T>(string query, object param)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<T>(
                query, param
            );

            return result.AsList();
        }

    }
}