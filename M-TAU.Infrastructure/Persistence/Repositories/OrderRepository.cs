using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;
using M_TAU.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Orders.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyCollection<Order>> ListAsync(CancellationToken cancellationToken = default)
        => await context.Orders.ToListAsync(cancellationToken);

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        await context.Orders.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        if (context.Entry(entity).State == EntityState.Detached)
            context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        context.Orders.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> ListByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default)
        => await context.Orders
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
}
