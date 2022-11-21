using System.Data;

namespace ApiSample.Persistence.Dapper.Blocks
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
