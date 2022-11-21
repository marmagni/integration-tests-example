namespace ApiSample.Persistence.Entities
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public string Name { get; set; }
    }
}
