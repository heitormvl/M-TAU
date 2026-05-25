using M_TAU.Application.Dtos.Catalog;
using M_TAU.Application.Services;
using M_TAU.Domain.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponseDto>>> GetAll(
        [FromQuery] ProductFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await productService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await productService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(
        [FromBody] ProductCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await productService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ProductCreateDto updateDto,
        CancellationToken cancellationToken)
    {
        await productService.UpdateAsync(id, updateDto, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] ProductStatus status,
        CancellationToken cancellationToken)
    {
        await productService.UpdateStatusAsync(id, status, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/photos")]
    public async Task<ActionResult<ProductResponseDto>> AddPhoto(
        Guid id,
        [FromBody] PhotoCreateDto photoDto,
        CancellationToken cancellationToken)
    {
        var result = await productService.AddPhotoAsync(id, photoDto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> RemovePhoto(
        Guid id,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        await productService.RemovePhotoAsync(id, photoId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
