using M_TAU.Application.Dtos.Catalog;
using M_TAU.Application.Dtos.Common;
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
    public async Task<ActionResult<PaginatedResult<ProductResponseDto>>> GetAll(
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

    [HttpPost("{id:guid}/photos/upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<ProductResponseDto>> UploadPhoto(
        Guid id,
        IFormFile file,
        [FromQuery] bool isMain,
        [FromServices] IWebHostEnvironment env,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "File is required." });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(file.ContentType))
            return BadRequest(new { message = "Unsupported image type." });

        var webRoot = env.WebRootPath;
        if (string.IsNullOrEmpty(webRoot))
            webRoot = Path.Combine(env.ContentRootPath, "wwwroot");

        var uploadsDir = Path.Combine(webRoot, "uploads", "products", id.ToString());
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext))
            ext = file.ContentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".jpg"
            };

        var filename = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsDir, filename);
        await using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream, cancellationToken);

        var publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{id}/{filename}";
        var result = await productService.AddPhotoAsync(id, new PhotoCreateDto(publicUrl, isMain), cancellationToken);
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
