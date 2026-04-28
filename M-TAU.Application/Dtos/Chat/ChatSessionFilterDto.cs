namespace M_TAU.Application.Dtos.Chat;

public record ChatSessionFilterDto(
    Guid? BuyerId = null,
    Guid? SellerId = null,
    Guid? ProductId = null,
    int? PageNumber = null,
    int? PageSize = null
);
