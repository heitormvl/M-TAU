using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using M_TAU.Application.Dtos.Admin;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller,Admin")]
public sealed class SellerController(IMetricsService metricsService) : ControllerBase
{
    [HttpGet("metrics")]
    public async Task<ActionResult<SellerMetricsDto>> GetMetrics(CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(sub, out var sellerId))
            return Unauthorized(new { message = "Invalid seller identity." });

        var metrics = await metricsService.GetSellerMetricsAsync(sellerId, cancellationToken);
        return Ok(metrics);
    }
}
