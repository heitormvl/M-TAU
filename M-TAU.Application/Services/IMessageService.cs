using M_TAU.Application.Dtos.Chat;
using M_TAU.Domain.Chat;

namespace M_TAU.Application.Services;

public interface IMessageService
{
    Task<IReadOnlyCollection<MessageResponseDto>> GetBySessionAsync(Guid chatSessionId, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Guid messageId, MessageStatus status, CancellationToken cancellationToken = default);
}
