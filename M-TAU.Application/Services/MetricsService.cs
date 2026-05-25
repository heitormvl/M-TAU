using M_TAU.Application.Dtos.Admin;
using M_TAU.Domain.Catalog;
using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Services;

public sealed class MetricsService(
    IUserRepository userRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IChatSessionRepository chatSessionRepository) : IMetricsService
{
    public async Task<AdminMetricsDto> GetAdminMetricsAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.ListAsync(cancellationToken);
        var products = await productRepository.ListAsync(cancellationToken);
        var orders = await orderRepository.ListAsync(cancellationToken);
        var sessions = await chatSessionRepository.ListAsync(cancellationToken);
        return new AdminMetricsDto(users.Count, products.Count, orders.Count, sessions.Count);
    }

    public async Task<SellerMetricsDto> GetSellerMetricsAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.ListBySellerAsync(sellerId, cancellationToken);
        var productIds = products.Select(p => p.Id).ToHashSet();
        var orders = await orderRepository.ListAsync(cancellationToken);
        var sellerOrders = orders.Where(o => productIds.Contains(o.ProductId)).ToList();

        var activeListings = products.Count(p => p.Status == ProductStatus.Active);
        var totalSales = sellerOrders.Count(o => o.Status == OrderStatus.Completed);
        var pendingOrders = sellerOrders.Count(o => o.Status is OrderStatus.Pending or OrderStatus.Confirmed);
        var revenue = sellerOrders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount);

        return new SellerMetricsDto(activeListings, totalSales, pendingOrders, revenue);
    }
}
