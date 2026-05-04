using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;
using M_TAU.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence.Repositories;

public sealed class FeedbackRepository(AppDbContext context) : IFeedbackRepository
{
    public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Feedbacks.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyCollection<Feedback>> ListAsync(CancellationToken cancellationToken = default)
        => await context.Feedbacks.ToListAsync(cancellationToken);

    public async Task AddAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        await context.Feedbacks.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        context.Feedbacks.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Feedback>> ListByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await context.Feedbacks
            .Where(f => f.OrderId == orderId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Feedback>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Feedbacks
            .Where(f => f.FromUserId == userId)
            .ToListAsync(cancellationToken);
}
