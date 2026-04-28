using M_TAU.Application.Dtos.Chat;

namespace M_TAU.Application.Services;

public interface IChatSessionService
{
    Task<ChatSessionResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChatSessionResponseDto>> GetAllAsync(ChatSessionFilterDto filter, CancellationToken cancellationToken = default);

    Task<ChatSessionResponseDto> CreateAsync(ChatSessionCreateDto createDto, CancellationToken cancellationToken = default);

    Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto, CancellationToken cancellationToken = default);
}
