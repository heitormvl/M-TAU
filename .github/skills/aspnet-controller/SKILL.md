---
name: aspnet-controller
description: "Use when generating an ASP.NET Core Web API Controller for an entity [NOME] in M-TAU.API. Triggers: controller, endpoint, REST, HttpGet, HttpPost, HttpPut, HttpDelete, API layer, ActionResult, CancellationToken."
argument-hint: "Entity name (e.g. Product, Order, User, ChatSession, Feedback)"
---

# ASP.NET Core Controller Generator — M-TAU.API

## When to Use
- Creating a new REST controller for a domain entity in `M-TAU.API/Controllers/`
- Exposing CRUD operations through HTTP endpoints
- Wiring a `M-TAU.Application` service interface to the API layer

## Conventions (M-TAU Project)

| Convention | Value |
|---|---|
| Namespace | `M_TAU.API.Controllers` |
| Base class | `ControllerBase` |
| Attributes | `[ApiController]`, `[Route("api/[controller]")]` |
| Constructor injection | `I{Nome}Service` from `M_TAU.Application.Services` |
| Input DTOs | `{Nome}CreateDto` for POST/PUT |
| Filter DTOs | `{Nome}FilterDto` for GET list |
| Output DTOs | `{Nome}ResponseDto` |
| Async pattern | All methods `async Task<ActionResult<...>>` with `CancellationToken` |

## Domain → Service → DTO Mapping

Each entity in M-TAU.Domain has a corresponding service interface in M-TAU.Application:

| Domain Entity | Service Interface | DTO Namespace |
|---|---|---|
| Product | `IProductService` | `M_TAU.Application.Dtos.Catalog` |
| Order | `IOrderService` | `M_TAU.Application.Dtos.Transaction` |
| Feedback | `IFeedbackService` | `M_TAU.Application.Dtos.Transaction` |
| User | `IUserService` | `M_TAU.Application.Dtos.Identity` |
| ChatSession | `IChatSessionService` | `M_TAU.Application.Dtos.Chat` |
| Message | `IMessageService` | `M_TAU.Application.Dtos.Chat` |

## Procedure

1. **Identify the entity** from the argument (e.g., `Product`, `Order`).
2. **Read the service interface** from `M-TAU.Application/Services/I{Nome}Service.cs` to discover available methods.
3. **Read the DTOs** from `M-TAU.Application/Dtos/{Domain}/` to confirm DTO names.
4. **Generate the controller** using the [template](./assets/ControllerTemplate.md) below.
5. **Save** to `M-TAU.API/Controllers/{Nome}Controller.cs`.

## HTTP Status Code Rules

| Scenario | Status Code |
|---|---|
| GET found | `200 Ok(dto)` |
| GET not found | `404 NotFound()` |
| POST created | `201 CreatedAtAction(nameof(GetById), new { id }, dto)` |
| POST invalid input | `400 BadRequest(ModelState)` — handled automatically by `[ApiController]` |
| PUT success | `204 NoContent()` |
| DELETE success | `204 NoContent()` |
| Not found in PUT/DELETE | `404 NotFound()` |

## Controller Template

```csharp
using M_TAU.Application.Dtos.{Domain};
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace M_TAU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class {Nome}Controller : ControllerBase
{
    private readonly I{Nome}Service _{camelNome}Service;

    public {Nome}Controller(I{Nome}Service {camelNome}Service)
    {
        _{camelNome}Service = {camelNome}Service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<{Nome}ResponseDto>>> GetAll(
        [FromQuery] {Nome}FilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _{camelNome}Service.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<{Nome}ResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _{camelNome}Service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<{Nome}ResponseDto>> Create(
        [FromBody] {Nome}CreateDto createDto,
        CancellationToken cancellationToken)
    {
        var result = await _{camelNome}Service.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] {Nome}CreateDto updateDto,
        CancellationToken cancellationToken)
    {
        await _{camelNome}Service.UpdateAsync(id, updateDto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _{camelNome}Service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
```

> **Note:** Adjust available methods to match what actually exists in `I{Nome}Service`. Not every entity has `Update` or `Delete`. Always read the service interface first (Step 2).

## Adaptation Notes

- If the service has `UpdateStatusAsync(Guid id, {Nome}Status status)`, add a `[HttpPatch("{id:guid}/status")]` endpoint that accepts `{Nome}Status` from `[FromBody]`.
- If the service has `AddPhotoAsync` / `RemovePhotoAsync`, add dedicated `[HttpPost("{id:guid}/photos")]` and `[HttpDelete("{id:guid}/photos/{photoId:guid}")]` endpoints.
- If there is **no** `GetByIdAsync` on the service, replace `CreatedAtAction` with `Created(string.Empty, result)`.
- Use `file-scoped namespace` (no braces around namespace).
