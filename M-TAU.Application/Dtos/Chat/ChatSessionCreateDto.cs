namespace M_TAU.Application.Dtos.Chat;

public record ChatSessionCreateDto(
    Guid BuyerId,
    Guid SellerId,
    Guid ProductId
);
