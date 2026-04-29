using M_TAU.Application.Dtos.Transaction;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class FeedbacksController(IFeedbackService feedbackService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FeedbackResponseDto>>> GetAll(
        [FromQuery] FeedbackFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await feedbackService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FeedbackResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await feedbackService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<FeedbackResponseDto>> Create(
        [FromBody] FeedbackCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await feedbackService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
