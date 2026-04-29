using M_TAU.Domain.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Rating)
            .IsRequired();

        builder.Property(f => f.Comment)
            .HasMaxLength(1000);

        builder.Property(f => f.FromUserId)
            .IsRequired();

        builder.Property(f => f.OrderId);
    }
}
