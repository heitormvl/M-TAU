using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Chat;

public record MessageCreateDto(
    [Required]
    Guid ChatSessionId,

    [Required]
    Guid SenderId,

    [Required]
    [StringLength(5000, MinimumLength = 1)]
    string Content
);
