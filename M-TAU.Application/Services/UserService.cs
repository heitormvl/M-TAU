using AutoMapper;
using M_TAU.Application.Dtos.Identity;
using M_TAU.Domain.Identity;
using M_TAU.Domain.Repositories;

namespace M_TAU.Application.Services;

public sealed class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    public async Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{id}' not found.");
        return mapper.Map<UserResponseDto>(user);
    }

    public async Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(UserFilterDto filter, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.ListAsync(cancellationToken);
        return mapper.Map<IReadOnlyCollection<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
        var user = new User(Guid.NewGuid(), createDto.Name, createDto.Email, passwordHash, createDto.Role);
        await userRepository.AddAsync(user, cancellationToken);
        return mapper.Map<UserResponseDto>(user);
    }

    public async Task UpdateNameAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{id}' not found.");
        user.SetName(name);
        await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task UpdateEmailAsync(Guid id, string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{id}' not found.");
        user.SetEmail(email);
        await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{id}' not found.");

        user.Anonymize();
        await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        return user;
    }
}
