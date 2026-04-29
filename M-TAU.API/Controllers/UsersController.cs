using M_TAU.Application.Dtos.Identity;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(
        [FromBody] UserCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await userService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserResponseDto>>> GetAll(
        [FromQuery] UserFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> UpdateName(
        Guid id,
        [FromBody] UpdateNameRequest request,
        CancellationToken cancellationToken)
    {
        await userService.UpdateNameAsync(id, request.Name, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/email")]
    public async Task<IActionResult> UpdateEmail(
        Guid id,
        [FromBody] UpdateEmailRequest request,
        CancellationToken cancellationToken)
    {
        await userService.UpdateEmailAsync(id, request.Email, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

public record UpdateNameRequest(string Name);
public record UpdateEmailRequest(string Email);
