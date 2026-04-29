using M_TAU.Application.Dtos.Transaction;
using M_TAU.Application.Services;
using M_TAU.Domain.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponseDto>>> GetAll(
        [FromQuery] OrderFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await orderService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await orderService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(
        [FromBody] OrderCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await orderService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] OrderStatus status,
        CancellationToken cancellationToken)
    {
        await orderService.UpdateStatusAsync(id, status, cancellationToken);
        return NoContent();
    }
}
