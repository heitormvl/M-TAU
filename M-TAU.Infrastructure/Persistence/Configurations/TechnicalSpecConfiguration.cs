using M_TAU.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class TechnicalSpecConfiguration : IEntityTypeConfiguration<TechnicalSpec>
{
    public void Configure(EntityTypeBuilder<TechnicalSpec> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Category)
            .IsRequired();

        builder.Property(t => t.Measures)
            .HasMaxLength(500);

        builder.Property(t => t.UsageTime)
            .HasMaxLength(200);
    }
}
