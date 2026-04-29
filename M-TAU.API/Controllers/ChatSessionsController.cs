using M_TAU.Application.Dtos.Chat;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ChatSessionsController(IChatSessionService chatSessionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ChatSessionResponseDto>>> GetAll(
        [FromQuery] ChatSessionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await chatSessionService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ChatSessionResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await chatSessionService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ChatSessionResponseDto>> Create(
        [FromBody] ChatSessionCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await chatSessionService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<MessageResponseDto>> SendMessage(
        Guid id,
        [FromBody] MessageCreateDto messageDto,
        CancellationToken cancellationToken)
    {
        var result = await chatSessionService.SendMessageAsync(messageDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }
}
