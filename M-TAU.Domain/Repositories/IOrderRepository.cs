using M_TAU.Domain.Transaction;

namespace M_TAU.Domain.Repositories;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IReadOnlyCollection<Order>> ListByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default);
}
