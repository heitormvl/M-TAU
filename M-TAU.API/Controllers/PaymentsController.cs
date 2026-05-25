using M_TAU.Application.Dtos.Payment;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [Authorize]
    [HttpPost("checkout")]
    public async Task<ActionResult<PaymentCheckoutResponseDto>> Checkout(
        [FromBody] PaymentCheckoutRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateCheckoutAsync(request.OrderId, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] PaymentWebhookDto webhook,
        CancellationToken cancellationToken)
    {
        await paymentService.HandleWebhookAsync(webhook, cancellationToken);
        return Ok();
    }
}
