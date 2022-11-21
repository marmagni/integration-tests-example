namespace ApiSample.Persistence.Entities
{
    public class Product
    {
        public Product()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }
}
