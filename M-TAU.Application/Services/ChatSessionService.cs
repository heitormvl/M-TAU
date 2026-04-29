using AutoMapper;
using M_TAU.Application.Dtos.Chat;
using M_TAU.Domain.Chat;
using M_TAU.Domain.Repositories;

namespace M_TAU.Application.Services;

public sealed class ChatSessionService(IChatSessionRepository chatSessionRepository, IMapper mapper) : IChatSessionService
{
    public async Task<ChatSessionResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await chatSessionRepository.GetWithMessagesAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"ChatSession '{id}' not found.");
        return mapper.Map<ChatSessionResponseDto>(session);
    }

    public async Task<IReadOnlyCollection<ChatSessionResponseDto>> GetAllAsync(ChatSessionFilterDto filter, CancellationToken cancellationToken = default)
    {
        var sessions = await chatSessionRepository.ListAsync(cancellationToken);
        return mapper.Map<IReadOnlyCollection<ChatSessionResponseDto>>(sessions);
    }

    public async Task<ChatSessionResponseDto> CreateAsync(ChatSessionCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var session = new ChatSession(Guid.NewGuid(), createDto.BuyerId, createDto.SellerId, createDto.ProductId);
        await chatSessionRepository.AddAsync(session, cancellationToken);
        return mapper.Map<ChatSessionResponseDto>(session);
    }

    public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto, CancellationToken cancellationToken = default)
    {
        var session = await chatSessionRepository.GetWithMessagesAsync(messageDto.ChatSessionId, cancellationToken)
            ?? throw new KeyNotFoundException($"ChatSession '{messageDto.ChatSessionId}' not found.");
        var message = new Message(Guid.NewGuid(), messageDto.ChatSessionId, messageDto.SenderId, messageDto.Content);
        session.AddMessage(message);
        await chatSessionRepository.UpdateAsync(session, cancellationToken);
        return mapper.Map<MessageResponseDto>(message);
    }
}
