using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderResponseDto(
    Guid Id,
    Guid BuyerId,
    Guid ProductId,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime OrderDate
);
