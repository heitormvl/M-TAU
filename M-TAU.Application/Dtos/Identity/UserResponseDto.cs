using M_TAU.Domain.Identity;

namespace M_TAU.Application.Dtos.Identity;

public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    UserType Role,
    DateTime CreatedAt
);
