using M_TAU.Domain.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ChatSessionId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(m => m.SentAt)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired();
    }
}
