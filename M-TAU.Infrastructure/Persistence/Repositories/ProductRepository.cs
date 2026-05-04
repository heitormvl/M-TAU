using M_TAU.Domain.Catalog;
using M_TAU.Domain.Repositories;
using M_TAU.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Products
            .Include(p => p.Photos)
            .Include(p => p.TechnicalSpec)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default)
        => await context.Products
            .Include(p => p.Photos)
            .Include(p => p.TechnicalSpec)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        if (context.Entry(entity).State == EntityState.Detached)
            context.Entry(entity).State = EntityState.Modified;
        foreach (var photo in entity.Photos)
        {
            if (context.Entry(photo).State == EntityState.Detached)
                context.Entry(photo).State = EntityState.Added;
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
    {
        context.Products.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> ListBySellerAsync(Guid sellerId, CancellationToken cancellationToken = default)
        => await context.Products
            .Where(p => p.SellerId == sellerId)
            .Include(p => p.Photos)
            .Include(p => p.TechnicalSpec)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Product>> ListByCategoryAsync(DisabilityCategory category, CancellationToken cancellationToken = default)
        => await context.Products
            .Where(p => p.TechnicalSpec != null && p.TechnicalSpec.Category == category)
            .Include(p => p.Photos)
            .Include(p => p.TechnicalSpec)
            .ToListAsync(cancellationToken);
}
