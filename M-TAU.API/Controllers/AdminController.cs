using System.Security.Claims;
using M_TAU.Application.Dtos.Admin;
using M_TAU.Application.Services;
using M_TAU.Domain.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class AdminController(
    IMetricsService metricsService,
    IProductService productService) : ControllerBase
{
    [HttpGet("metrics")]
    public async Task<ActionResult<AdminMetricsDto>> GetMetrics(CancellationToken cancellationToken)
        => Ok(await metricsService.GetAdminMetricsAsync(cancellationToken));

    [HttpPatch("products/{id:guid}/moderate")]
    public async Task<IActionResult> ModerateProduct(
        Guid id,
        [FromBody] ModerateProductRequest body,
        CancellationToken cancellationToken)
    {
        await productService.UpdateStatusAsync(id, body.Status, cancellationToken);
        return NoContent();
    }
}

public sealed record ModerateProductRequest(ProductStatus Status);
