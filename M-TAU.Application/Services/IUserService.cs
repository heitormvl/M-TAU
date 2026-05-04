using M_TAU.Application.Dtos.Identity;
using M_TAU.Domain.Identity;

namespace M_TAU.Application.Services;

public interface IUserService
{
    Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(UserFilterDto filter, CancellationToken cancellationToken = default);

    Task<UserResponseDto> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default);

    Task UpdateNameAsync(Guid id, string name, CancellationToken cancellationToken = default);

    Task UpdateEmailAsync(Guid id, string email, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}
