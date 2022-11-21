using ApiSample.Persistence.Entities;

namespace TestServer.Mocks
{
    public class EntityMocks
    {
        public static Category GetCategory()
        {
            return new Category
            {
                Name = "Categoria teste"
            };
        }

        public static Product GetProduct(Guid categoryId)
        {
            return new Product
            {                
                Available = true,
                Price = 1245.50m,
                CategoryId = categoryId,
                Name = "Produto de teste",
                Description = "Descrição teste",
            };
        }
    }
}
