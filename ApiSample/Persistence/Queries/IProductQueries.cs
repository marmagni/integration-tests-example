using ApiSample.Persistence.Queries.DTOs;
using ApiSample.Persistence.Queries.Filters;

namespace ApiSample.Persistence.Queries
{
    public interface IProductQueries
    {
        Task<List<ProductDTO>> Search(SearchProductFilters filters);
    }
}
