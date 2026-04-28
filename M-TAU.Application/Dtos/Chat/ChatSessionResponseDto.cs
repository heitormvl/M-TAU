namespace M_TAU.Application.Dtos.Chat;

public record ChatSessionResponseDto(
    Guid Id,
    Guid BuyerId,
    Guid SellerId,
    Guid ProductId,
    DateTime CreatedAt,
    IReadOnlyCollection<MessageResponseDto>? Messages = null
);
