namespace ApiSample.Persistence.Dapper
{
    public interface IDapperQuery
    {
        Task<List<T>> QueryAsync<T>(string query, object param);
    }
}
