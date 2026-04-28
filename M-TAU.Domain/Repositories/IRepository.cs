namespace M_TAU.Domain.Repositories;

public interface IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TEntity>> ListAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
