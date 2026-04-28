using M_TAU.Domain.Chat;

namespace M_TAU.Domain.Repositories;

public interface IChatSessionRepository : IRepository<ChatSession, Guid>
{
    Task<IReadOnlyCollection<ChatSession>> ListByParticipantAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ChatSession?> GetWithMessagesAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
