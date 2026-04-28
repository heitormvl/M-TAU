using M_TAU.Domain.Identity;

namespace M_TAU.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
