using M_TAU.Application.Dtos.Payment;
using M_TAU.Domain.Catalog;
using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;
using Microsoft.Extensions.Configuration;

namespace M_TAU.Application.Services;

public sealed class PaymentService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IConfiguration configuration) : IPaymentService
{
    public async Task<PaymentCheckoutResponseDto> CreateCheckoutAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{orderId}' not found.");

        if (order.Status == OrderStatus.Completed)
            throw new InvalidOperationException("Order is already paid.");

        var provider = configuration["Payments:Provider"] ?? "MercadoPago-Sandbox";
        var baseUrl = configuration["Payments:CheckoutBaseUrl"] ?? "https://sandbox.mercadopago.com.br/checkout/v1/redirect";

        order.UpdateStatus(OrderStatus.Confirmed);
        await orderRepository.UpdateAsync(order, cancellationToken);

        var checkoutUrl = $"{baseUrl}?pref_id={order.Id}&order={order.Id}";
        return new PaymentCheckoutResponseDto(order.Id, checkoutUrl, provider);
    }

    public async Task HandleWebhookAsync(PaymentWebhookDto webhook, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(webhook.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{webhook.OrderId}' not found.");

        var normalized = (webhook.Status ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized is "approved" or "paid" or "completed")
        {
            order.UpdateStatus(OrderStatus.Completed);
            await orderRepository.UpdateAsync(order, cancellationToken);

            var product = await productRepository.GetByIdAsync(order.ProductId, cancellationToken);
            if (product is not null && product.Status != ProductStatus.Sold)
            {
                product.UpdateStatus(ProductStatus.Sold);
                await productRepository.UpdateAsync(product, cancellationToken);
            }
        }
        else if (normalized is "cancelled" or "rejected")
        {
            order.UpdateStatus(OrderStatus.Cancelled);
            await orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
