using M_TAU.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.SellerId)
            .IsRequired();

        builder.HasMany(p => p.Photos)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.TechnicalSpec)
            .WithOne()
            .HasForeignKey<TechnicalSpec>("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
