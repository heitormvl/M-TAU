using M_TAU.Domain.Identity;

namespace M_TAU.Application.Dtos.Identity;

public record UserFilterDto(
    string? Name = null,
    string? Email = null,
    UserType? Role = null,
    int? PageNumber = null,
    int? PageSize = null
);
