namespace ApiSample.Persistence.Queries.Filters
{
    public class SearchProductFilters
    {
        public string? Name { get; set; }
        public Guid?[] Categories { get; set; } = new Guid?[0];
    }
}
