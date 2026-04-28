using M_TAU.Domain.Chat;

namespace M_TAU.Application.Dtos.Chat;

public record MessageResponseDto(
    Guid Id,
    Guid ChatSessionId,
    Guid SenderId,
    string Content,
    DateTime SentAt,
    MessageStatus Status
);
