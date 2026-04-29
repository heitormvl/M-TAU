using AutoMapper;
using M_TAU.Application.Dtos.Chat;
using M_TAU.Domain.Chat;
using M_TAU.Domain.Repositories;

namespace M_TAU.Application.Services;

public sealed class MessageService(IChatSessionRepository chatSessionRepository, IMapper mapper) : IMessageService
{
    public async Task<IReadOnlyCollection<MessageResponseDto>> GetBySessionAsync(Guid chatSessionId, CancellationToken cancellationToken = default)
    {
        var session = await chatSessionRepository.GetWithMessagesAsync(chatSessionId, cancellationToken)
            ?? throw new KeyNotFoundException($"ChatSession '{chatSessionId}' not found.");
        return mapper.Map<IReadOnlyCollection<MessageResponseDto>>(session.Messages);
    }

    public async Task UpdateStatusAsync(Guid messageId, MessageStatus status, CancellationToken cancellationToken = default)
    {
        var sessions = await chatSessionRepository.ListAsync(cancellationToken);
        Message? target = null;
        ChatSession? ownerSession = null;

        foreach (var session in sessions)
        {
            var full = await chatSessionRepository.GetWithMessagesAsync(session.Id, cancellationToken);
            target = full?.Messages.FirstOrDefault(m => m.Id == messageId);
            if (target is not null)
            {
                ownerSession = full;
                break;
            }
        }

        if (target is null || ownerSession is null)
            throw new KeyNotFoundException($"Message '{messageId}' not found.");

        target.UpdateStatus(status);
        await chatSessionRepository.UpdateAsync(ownerSession, cancellationToken);
    }
}
