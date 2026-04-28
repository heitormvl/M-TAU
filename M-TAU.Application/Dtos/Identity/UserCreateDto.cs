using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Identity;

public record UserCreateDto(
    [Required]
    [StringLength(120, MinimumLength = 2)]
    string Name,

    [Required]
    [EmailAddress]
    [StringLength(256)]
    string Email,

    [Required]
    [StringLength(256, MinimumLength = 8)]
    string Password,

    [Required]
    M_TAU.Domain.Identity.UserType Role
);
