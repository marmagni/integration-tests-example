using ApiSample.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiSample.Persistence.EFCore
{
    public class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions options) 
            : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
    }
}
