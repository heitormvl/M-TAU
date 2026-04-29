using M_TAU.Domain.Transaction;

namespace M_TAU.Domain.Repositories;

public interface IFeedbackRepository : IRepository<Feedback, Guid>
{
    Task<IReadOnlyCollection<Feedback>> ListByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Feedback>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
