using Microsoft.EntityFrameworkCore;
using ApiSample.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSample.Persistence.EFCore.Mappings
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product").HasKey(c => c.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            
            builder.Property(x => x.Name).HasMaxLength(150).HasColumnType("varchar").IsRequired();
            builder.Property(x => x.Description).HasMaxLength(512).HasColumnType("varchar");
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Available).IsRequired();

            builder.HasOne<Category>().WithOne()
                    .HasPrincipalKey<Category>(x => x.Id)
                    .HasForeignKey<Product>(x => x.CategoryId);
        }
    }
}
