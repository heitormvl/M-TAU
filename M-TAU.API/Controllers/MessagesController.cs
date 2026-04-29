using M_TAU.Application.Dtos.Chat;
using M_TAU.Application.Services;
using M_TAU.Domain.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MessageResponseDto>>> GetBySession(
        [FromQuery] Guid sessionId,
        CancellationToken cancellationToken)
    {
        var result = await messageService.GetBySessionAsync(sessionId, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] MessageStatus status,
        CancellationToken cancellationToken)
    {
        await messageService.UpdateStatusAsync(id, status, cancellationToken);
        return NoContent();
    }
}
