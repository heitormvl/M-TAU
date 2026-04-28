using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderFilterDto(
    Guid? BuyerId = null,
    Guid? ProductId = null,
    OrderStatus? Status = null,
    int? PageNumber = null,
    int? PageSize = null
);
