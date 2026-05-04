using M_TAU.Domain.Chat;
using M_TAU.Domain.Repositories;
using M_TAU.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence.Repositories;

public sealed class ChatSessionRepository(AppDbContext context) : IChatSessionRepository
{
    public async Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.ChatSessions.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyCollection<ChatSession>> ListAsync(CancellationToken cancellationToken = default)
        => await context.ChatSessions.ToListAsync(cancellationToken);

    public async Task AddAsync(ChatSession entity, CancellationToken cancellationToken = default)
    {
        await context.ChatSessions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ChatSession entity, CancellationToken cancellationToken = default)
    {
        foreach (var message in entity.Messages)
        {
            if (context.Entry(message).State == EntityState.Detached)
                context.Entry(message).State = EntityState.Added;
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ChatSession entity, CancellationToken cancellationToken = default)
    {
        context.ChatSessions.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChatSession>> ListByParticipantAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.ChatSessions
            .Where(s => s.BuyerId == userId || s.SellerId == userId)
            .ToListAsync(cancellationToken);

    public async Task<ChatSession?> GetWithMessagesAsync(Guid sessionId, CancellationToken cancellationToken = default)
        => await context.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
}
