using M_TAU.Domain.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.BuyerId)
            .IsRequired();

        builder.Property(c => c.SellerId)
            .IsRequired();

        builder.Property(c => c.ProductId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(m => m.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
