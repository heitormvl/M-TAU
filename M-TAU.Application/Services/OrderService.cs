using AutoMapper;
using M_TAU.Application.Dtos.Transaction;
using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Services;

public sealed class OrderService(IOrderRepository orderRepository, IMapper mapper) : IOrderService
{
    public async Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{id}' not found.");
        return mapper.Map<OrderResponseDto>(order);
    }

    public async Task<IReadOnlyCollection<OrderResponseDto>> GetAllAsync(OrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Order> orders;
        if (filter.BuyerId.HasValue)
            orders = await orderRepository.ListByBuyerAsync(filter.BuyerId.Value, cancellationToken);
        else
            orders = await orderRepository.ListAsync(cancellationToken);

        var query = orders.AsEnumerable();
        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);
        if (filter.ProductId.HasValue)
            query = query.Where(o => o.ProductId == filter.ProductId.Value);

        return mapper.Map<IReadOnlyCollection<OrderResponseDto>>(query.ToList());
    }

    public async Task<OrderResponseDto> CreateAsync(OrderCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var order = new Order(Guid.NewGuid(), createDto.BuyerId, createDto.ProductId, createDto.TotalAmount);
        await orderRepository.AddAsync(order, cancellationToken);
        return mapper.Map<OrderResponseDto>(order);
    }

    public async Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{id}' not found.");
        order.UpdateStatus(status);
        await orderRepository.UpdateAsync(order, cancellationToken);
    }
}
