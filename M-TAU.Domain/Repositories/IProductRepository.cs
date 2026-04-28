using M_TAU.Domain.Catalog;

namespace M_TAU.Domain.Repositories;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<IReadOnlyCollection<Product>> ListBySellerAsync(Guid sellerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> ListByCategoryAsync(DisabilityCategory category, CancellationToken cancellationToken = default);
}
