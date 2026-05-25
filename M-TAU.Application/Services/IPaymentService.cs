using M_TAU.Application.Dtos.Payment;

namespace M_TAU.Application.Services;

public interface IPaymentService
{
    Task<PaymentCheckoutResponseDto> CreateCheckoutAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task HandleWebhookAsync(PaymentWebhookDto webhook, CancellationToken cancellationToken = default);
}
