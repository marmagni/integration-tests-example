using ApiSample.Persistence.Dapper;
using ApiSample.Persistence.Queries.DTOs;
using ApiSample.Persistence.Queries.Filters;

namespace ApiSample.Persistence.Queries
{
    public class ProductQueries : IProductQueries
    {
        readonly IDapperQuery _dapperQuery;
        public ProductQueries(IDapperQuery dapperQuery)
        {
            _dapperQuery = dapperQuery;
        }

        public async Task<List<ProductDTO>> Search(SearchProductFilters filters)
        {
            filters ??= new();

            var query = @"SELECT p.Id, p.Name, p.Description, p.Price, p.CategoryId, c.Name as CategoryName
                    FROM product p INNER JOIN category c ON p.CategoryId = c.Id
                    WHERE p.Available = 1";

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query += "AND p.Name like '%'+@Name+'%'";
            }

            if (filters.Categories.Any())
            {
                query += "AND p.CategoryId IN @Categories";
            }

            var result = await _dapperQuery.QueryAsync<ProductDTO>(query, filters);

            return result;
        }
    }
}
