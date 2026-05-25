using M_TAU.Application.Dtos.Common;
using M_TAU.Application.Dtos.Transaction;
using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Services;

public interface IOrderService
{
    Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedResult<OrderResponseDto>> GetAllAsync(OrderFilterDto filter, CancellationToken cancellationToken = default);

    Task<OrderResponseDto> CreateAsync(OrderCreateDto createDto, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default);
}
