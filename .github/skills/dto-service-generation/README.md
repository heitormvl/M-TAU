# DTO and Service Interface Generation Skill

This skill provides a complete workflow for generating **Data Transfer Objects (DTOs)** and **async Service Interfaces** for M-TAU.Application based on domain entities from M-TAU.Domain.

## What This Skill Does

✅ Generates **3 DTO types per entity** (Create, Response, Filter) as immutable C# Records  
✅ Creates **async Service Interfaces** with `Task`-based method signatures  
✅ Enforces **M-TAU project conventions** (namespaces, DataAnnotations, CancellationToken)  
✅ Complements the `csharp-domain-models` skill for full Application layer scaffolding  
✅ Includes **automation scripts** (PowerShell) and **templates** for quick generation  

## How to Use

### Option 1: Use as Slash Command (Recommended)
```
/dto-service-generation Generate Product DTOs with properties: Title, Price, Description, Status, SellerId
```

### Option 2: Use PowerShell Script for Batch Generation
```powershell
cd M-TAU
.\\.github\skills\dto-service-generation\scripts\code-generator.ps1 -Entity "Product" -Namespace "Catalog" -Properties "Title:string:true,Price:decimal:true,Description:string:false"
```

### Option 3: Manual Template-Based Generation
1. Copy `./assets/dto-record-template.cs` → `M-TAU.Application/Dtos/{Namespace}/{Entity}*Dto.cs`
2. Copy `./assets/service-interface-template.cs` → `M-TAU.Application/Services/I{Entity}Service.cs`
3. Customize properties and methods

## Folder Structure

```
.github/skills/dto-service-generation/
├── SKILL.md                          # Main skill documentation (read this first!)
├── EXAMPLES.md                       # Real-world examples (Product, Order)
├── README.md                         # This file
├── assets/
│   ├── dto-record-template.cs       # DTO template (Create, Response, Filter)
│   └── service-interface-template.cs # Service Interface template
└── scripts/
    └── code-generator.ps1           # PowerShell automation script
```

## Key Principles

| Principle | Description |
|-----------|-------------|
| **Records for DTOs** | Immutable, structural equality, memory-efficient `record` syntax |
| **Async by Default** | All service methods return `Task<T>` or `Task` |
| **3-DTO Pattern** | Separate concerns: Create (input), Response (output), Filter (search) |
| **Domain-Aware** | Custom methods extracted from domain entity operations |
| **Validation Ready** | DataAnnotations for input DTOs (Required, StringLength, Range, etc.) |
| **Type Safe** | `IReadOnlyCollection<T>` for collections, nullable annotations explicit |

## Quick Start Example

### Domain Entity (M-TAU.Domain.Catalog)
```csharp
public class Product : EntityBase<Guid>
{
    public string Title { get; private set; }
    public decimal Price { get; private set; }
    public ProductStatus Status { get; private set; }
}
```

### Generated DTOs (M-TAU.Application.Dtos.Catalog)
```csharp
public record ProductCreateDto(
    [Required][StringLength(120, MinimumLength = 3)] string Title,
    [Range(0.01, double.MaxValue)] decimal Price
);

public record ProductResponseDto(Guid Id, string Title, decimal Price, DateTime CreatedAt);

public record ProductFilterDto(
    string? Title = null,
    decimal? PriceMin = null,
    [Range(1, int.MaxValue)] int? PageNumber = null
);
```

### Generated Service Interface (M-TAU.Application.Services)
```csharp
public interface IProductService
{
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductResponseDto>> FilterAsync(ProductFilterDto filter, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, ProductCreateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

## Next Steps After Generation

1. **Review generated files** for correctness
2. **Add custom methods** to Service Interface (domain-specific operations)
3. **Create Mappers** (AutoMapper or Mapster) to convert Domain ↔ DTO
4. **Implement Services** in `M-TAU.Application/Services/Implementation/`
5. **Register in DI** (Program.cs or Startup)
6. **Create Controllers/Endpoints** in M-TAU.API

## Related Skills

- **`csharp-domain-models`** – Generate domain entities and repositories. Use this FIRST to create the domain layer, then use `dto-service-generation` to scaffold the Application layer.

## Documentation Links

- [SKILL.md](./SKILL.md) – Full procedure and checklist
- [EXAMPLES.md](./EXAMPLES.md) – Real-world examples (Product, Order, Message)
- [C# Records Docs](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records)
- [DataAnnotations Docs](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannnotations)
- [Async Patterns](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)

---

**Questions?** Review [SKILL.md](./SKILL.md) for the complete step-by-step workflow and quality checklist.
