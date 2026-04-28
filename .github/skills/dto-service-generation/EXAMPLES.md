# DTO and Service Interface Examples

## Real-World Example: Product Entity

### Domain Entity (M-TAU.Domain)
```csharp
// M-TAU.Domain/Catalog/Product.cs
public class Product : EntityBase<Guid>
{
    [Required]
    [StringLength(120)]
    public string Title { get; private set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; private set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; private set; }

    [Required]
    public ProductStatus Status { get; private set; }

    [Required]
    public Guid SellerId { get; private set; }

    public TechnicalSpec? TechnicalSpec { get; private set; }
    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();

    // ... business methods
}
```

### Generated DTOs (M-TAU.Application)

#### ProductCreateDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Catalog;

/// <summary>
/// DTO for creating a new Product.
/// Client provides: Title, Price, Description.
/// Excluded: Id (generated), SellerId (from auth context), Status (defaults to Active), CreatedAt/UpdatedAt.
/// </summary>
public record ProductCreateDto(
    [Required]
    [StringLength(120, MinimumLength = 3)]
    string Title,
    
    [Range(0.01, double.MaxValue)]
    decimal Price,
    
    [StringLength(1000)]
    string? Description = null
);
```

#### ProductResponseDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Catalog;

/// <summary>
/// DTO for reading Product data.
/// Includes: All properties + Id + timestamps.
/// </summary>
public record ProductResponseDto(
    [Required]
    Guid Id,
    
    [Required]
    string Title,
    
    string? Description,
    
    [Required]
    decimal Price,
    
    [Required]
    string Status, // ProductStatus enum name
    
    [Required]
    Guid SellerId,
    
    DateTime CreatedAt,
    
    DateTime? UpdatedAt = null
);
```

#### ProductFilterDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Catalog;

/// <summary>
/// DTO for filtering Products.
/// All properties optional for flexible searching.
/// </summary>
public record ProductFilterDto(
    string? Title = null,
    
    decimal? PriceMin = null,
    
    decimal? PriceMax = null,
    
    string? Status = null,
    
    Guid? SellerId = null,
    
    [Range(1, int.MaxValue)]
    int? PageNumber = null,
    
    [Range(1, 100)]
    int? PageSize = null
);
```

### Generated Service Interface

#### IProductService.cs
```csharp
using M_TAU.Application.Dtos.Catalog;

namespace M_TAU.Application.Services;

/// <summary>
/// IProductService provides application-layer operations for Product management.
/// All methods are asynchronous (Task-based) and support CancellationToken.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves a single Product by its unique Id.
    /// </summary>
    Task<ProductResponseDto> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Products.
    /// </summary>
    Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters Products based on criteria (Title, Price range, Status, Seller, etc.).
    /// Supports pagination.
    /// </summary>
    Task<IReadOnlyCollection<ProductResponseDto>> FilterAsync(
        ProductFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new Product.
    /// </summary>
    Task<ProductResponseDto> CreateAsync(
        ProductCreateDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing Product.
    /// </summary>
    Task UpdateAsync(
        Guid id,
        ProductCreateDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a Product by its Id.
    /// </summary>
    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Domain-specific: Retrieve all Products by a specific Seller.
    /// This maps to the domain repository method ListBySellerAsync.
    /// </summary>
    Task<IReadOnlyCollection<ProductResponseDto>> ListBySellerAsync(
        Guid sellerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Domain-specific: Retrieve all Products in a Disability Category.
    /// This maps to the domain repository method ListByCategoryAsync.
    /// </summary>
    Task<IReadOnlyCollection<ProductResponseDto>> ListByCategoryAsync(
        string disabilityCategory,
        CancellationToken cancellationToken = default);
}
```

---

## Real-World Example: Order Entity

### Domain Entity (M-TAU.Domain)
```csharp
// M-TAU.Domain/Transaction/Order.cs
public class Order : EntityBase<Guid>
{
    [Required]
    public Guid BuyerId { get; private set; }

    [Required]
    public Guid ProductId { get; private set; }

    [Required]
    public DateTime OrderDate { get; private set; }

    [Required]
    public OrderStatus Status { get; private set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; private set; }

    // ... methods: UpdateStatus, SetTotalAmount
}
```

### Generated DTOs and Service Interface

#### OrderCreateDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderCreateDto(
    [Required]
    Guid BuyerId,
    
    [Required]
    Guid ProductId,
    
    [Range(0.01, double.MaxValue)]
    decimal TotalAmount
);
```

#### OrderResponseDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderResponseDto(
    [Required]
    Guid Id,
    
    [Required]
    Guid BuyerId,
    
    [Required]
    Guid ProductId,
    
    [Required]
    DateTime OrderDate,
    
    [Required]
    string Status, // OrderStatus enum
    
    [Required]
    decimal TotalAmount,
    
    DateTime CreatedAt,
    
    DateTime? UpdatedAt = null
);
```

#### OrderFilterDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderFilterDto(
    Guid? BuyerId = null,
    
    Guid? ProductId = null,
    
    string? Status = null,
    
    DateTime? OrderDateFrom = null,
    
    DateTime? OrderDateTo = null,
    
    [Range(1, int.MaxValue)]
    int? PageNumber = null,
    
    [Range(1, 100)]
    int? PageSize = null
);
```

#### IOrderService.cs
```csharp
using M_TAU.Application.Dtos.Transaction;

namespace M_TAU.Application.Services;

public interface IOrderService
{
    Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderResponseDto>> FilterAsync(OrderFilterDto filter, CancellationToken cancellationToken = default);
    Task<OrderResponseDto> CreateAsync(OrderCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, OrderCreateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Domain-specific: Get all Orders by a specific Buyer.
    /// </summary>
    Task<IReadOnlyCollection<OrderResponseDto>> GetByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Domain-specific: Update Order status (Pending → Completed, Cancelled, etc.).
    /// </summary>
    Task UpdateStatusAsync(Guid orderId, string newStatus, CancellationToken cancellationToken = default);
}
```

---

## Common Patterns and Anti-patterns

### ✓ GOOD: Separate DTOs for Different Concerns

```csharp
// Good: 3 focused DTOs
public record ProductCreateDto(string Title, decimal Price);
public record ProductResponseDto(Guid Id, string Title, decimal Price, DateTime CreatedAt);
public record ProductFilterDto(string? Title = null, decimal? PriceMin = null, int? PageNumber = null);
```

### ✗ BAD: Single DTO for Everything

```csharp
// Bad: Conflates concerns, exposes more than needed
public record ProductDto(
    Guid? Id, // Why optional on Create?
    string Title,
    decimal Price,
    string Status,
    Guid SellerId,
    DateTime? CreatedAt = null, // Confusing on Create
    DateTime? UpdatedAt = null  // Can't be on Create
);
```

### ✓ GOOD: Domain-Specific Methods in Service

```csharp
public interface IProductService
{
    // Standard CRUD
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    // Domain-specific operations from M_TAU.Domain
    Task<IReadOnlyCollection<ProductResponseDto>> ListBySellerAsync(Guid sellerId, CancellationToken ct = default);
    Task<IReadOnlyCollection<ProductResponseDto>> ListByCategoryAsync(string category, CancellationToken ct = default);
}
```

### ✗ BAD: Generic "Search" Method with Too Many Overloads

```csharp
// Bad: Unclear semantics, mixes concerns
public interface IProductService
{
    Task<ProductResponseDto> SearchAsync(string? query);
    Task<ProductResponseDto> SearchAsync(Guid sellerId);
    Task<ProductResponseDto> SearchAsync(string category, int page);
}
```

### ✓ GOOD: Always Use Task/Task<T> for Async

```csharp
public interface IProductService
{
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### ✗ BAD: Mixing sync and async

```csharp
// Bad: Inconsistent patterns
public interface IProductService
{
    ProductResponseDto GetById(Guid id); // Sync?
    Task<ProductResponseDto> GetByIdAsync(Guid id); // Async?
    async Task<ProductResponseDto> GetAllAsync(); // Redundant 'async' keyword
}
```

---

## Mapping Strategy (Next Step After DTOs)

Once DTOs and Service Interfaces are created, implement mappers to convert between domain entities and DTOs. Common approaches:

### AutoMapper
```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponseDto>().ReverseMap();
        CreateMap<ProductCreateDto, Product>();
    }
}
```

### Mapster
```csharp
public static class ProductMapper
{
    public static ProductResponseDto ToDto(this Product product)
        => new(product.Id, product.Title, product.Description, product.Price, product.Status.ToString(), product.SellerId, product.CreatedAt, product.UpdatedAt);
    
    public static Product ToEntity(this ProductCreateDto dto, Guid sellerId)
        => new(Guid.NewGuid(), dto.Title, dto.Price, sellerId) { /*set other fields*/ };
}
```

### Manual Mapping
```csharp
public ProductResponseDto MapToDto(Product entity)
    => new(
        entity.Id,
        entity.Title,
        entity.Description,
        entity.Price,
        entity.Status.ToString(),
        entity.SellerId,
        entity.CreatedAt,
        entity.UpdatedAt
    );
```
