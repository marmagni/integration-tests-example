namespace ApiSample.Controllers.Request
{
    public class CreateProductRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }
}
