---
name: efcore-infrastructure
description: "Use when generating EF Core DbContext, entity configurations (Fluent API), repository implementations, and DI registration for M-TAU.Infrastructure. Triggers: DbContext, AppDbContext, Fluent API, entity configuration, IEntityTypeConfiguration, repository implementation, AddDbContext, infrastructure DI."
argument-hint: "Describe which entity or layer to target, or leave blank to generate everything"
---

# EF Core Infrastructure Generation

Generates the full EF Core infrastructure layer for **M-TAU.Infrastructure**:
- `AppDbContext` with all `DbSet<>` properties
- One `IEntityTypeConfiguration<T>` class per entity (Fluent API)
- Concrete repository implementations for each domain interface
- `DependencyInjection.cs` extension for `IServiceCollection`

## Project Conventions

| Convention | Value |
|---|---|
| Root namespace | `M_TAU.Infrastructure` |
| Target framework | net10.0 |
| File-scoped namespaces | Yes |
| Nullable | enabled |
| EF Core provider | SqlServer (`Microsoft.EntityFrameworkCore.SqlServer`) |
| Domain namespace | `M_TAU.Domain` |
| Application namespace | `M_TAU.Application` |

## Entity Inventory

All entities inherit `EntityBase<Guid>` (Id is Guid PK).

| Entity | Namespace | Notes |
|---|---|---|
| `User` | `M_TAU.Domain.Identity` | Email max 256, unique index |
| `Product` | `M_TAU.Domain.Catalog` | Has TechnicalSpec (1-1), Photos (1-N), SellerId FK to User |
| `TechnicalSpec` | `M_TAU.Domain.Catalog` | Owned or separate table, FK to Product |
| `Photo` | `M_TAU.Domain.Catalog` | FK to Product |
| `ChatSession` | `M_TAU.Domain.Chat` | BuyerId/SellerId/ProductId are FK-like Guids |
| `Message` | `M_TAU.Domain.Chat` | FK to ChatSession |
| `Order` | `M_TAU.Domain.Transaction` | BuyerId/ProductId are FK-like Guids |
| `Feedback` | `M_TAU.Domain.Transaction` | OrderId nullable |

## Procedure

### 1. Read existing domain entities

Before generating, read:
- `M-TAU.Domain/Common/EntityBase.cs`
- Entity files in `Catalog/`, `Chat/`, `Identity/`, `Transaction/`
- Repository interfaces in `M-TAU.Domain/Repositories/`

### 2. Create `AppDbContext`

File: `M-TAU.Infrastructure/Persistence/AppDbContext.cs`

```csharp
using M_TAU.Domain.Catalog;
using M_TAU.Domain.Chat;
using M_TAU.Domain.Identity;
using M_TAU.Domain.Transaction;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<TechnicalSpec> TechnicalSpecs => Set<TechnicalSpec>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

### 3. Create Entity Configurations (Fluent API)

One file per entity in `M-TAU.Infrastructure/Persistence/Configurations/`.

**Rules:**
- Implement `IEntityTypeConfiguration<TEntity>`
- Use `HasKey(e => e.Id)` explicitly
- Map all `[MaxLength]` annotations as `.HasMaxLength()` in Fluent API
- Map `[Required]` as `.IsRequired()`
- Map `decimal` columns with `.HasPrecision(18, 2)`
- Map enums as `int` by default (no conversion unless specified)
- Map navigation collections with `.WithMany()` / `.HasForeignKey()`
- Map 1-1 optional relationships with `.WithOne().HasForeignKey()`

**Reference pattern:**
```csharp
namespace M_TAU.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.HasMany(p => p.Photos)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.TechnicalSpec)
            .WithOne()
            .HasForeignKey<TechnicalSpec>("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 4. Create Repository Implementations

One file per repository interface in `M-TAU.Infrastructure/Persistence/Repositories/`.

**Base pattern:**
```csharp
namespace M_TAU.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Products.FindAsync([id], ct);

    public async Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken ct = default)
        => await context.Products.ToListAsync(ct);

    public async Task AddAsync(Product entity, CancellationToken ct = default)
        => await context.Products.AddAsync(entity, ct);

    public Task UpdateAsync(Product entity, CancellationToken ct = default)
    {
        context.Products.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product entity, CancellationToken ct = default)
    {
        context.Products.Remove(entity);
        return Task.CompletedTask;
    }

    // entity-specific methods...
}
```

> Note: Do NOT call `SaveChangesAsync` inside repositories. That is the Unit of Work responsibility (handled via `AppDbContext` injected in services or via a UoW wrapper).

### 5. Create `DependencyInjection.cs`

File: `M-TAU.Infrastructure/DependencyInjection.cs`

```csharp
using M_TAU.Domain.Repositories;
using M_TAU.Infrastructure.Persistence;
using M_TAU.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace M_TAU.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

        return services;
    }
}
```

### 6. Register in `Program.cs` (API)

Add to `M-TAU.API/Program.cs`:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

Add connection string to `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MTAU;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 7. Remove placeholder

Delete or empty `M-TAU.Infrastructure/Class1.cs`.

## Quality Checklist

- [ ] `AppDbContext` has a `DbSet<>` for every entity
- [ ] `ApplyConfigurationsFromAssembly` is used (no manual `modelBuilder.Entity<>` calls in `OnModelCreating`)
- [ ] Every entity configuration is in its own file under `Configurations/`
- [ ] `decimal` properties use `HasPrecision(18, 2)`
- [ ] `Email` on `User` has a unique index
- [ ] All repository interfaces from `M_TAU.Domain.Repositories` are implemented and registered
- [ ] `DependencyInjection.cs` uses `AddScoped` for all repositories
- [ ] No `SaveChangesAsync` inside repositories
- [ ] `Program.cs` calls `AddInfrastructure`
