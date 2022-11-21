using Microsoft.EntityFrameworkCore;
using ApiSample.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSample.Persistence.EFCore.EntityConfigs
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("category").HasKey(c => c.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Name).HasMaxLength(150).HasColumnType("varchar").IsRequired();
        }
    }
}
