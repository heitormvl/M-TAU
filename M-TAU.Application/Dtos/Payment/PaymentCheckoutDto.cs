namespace M_TAU.Application.Dtos.Payment;

public sealed record PaymentCheckoutRequestDto(Guid OrderId);

public sealed record PaymentCheckoutResponseDto(Guid OrderId, string CheckoutUrl, string Provider);

public sealed record PaymentWebhookDto(Guid OrderId, string Status, string? ExternalReference);
