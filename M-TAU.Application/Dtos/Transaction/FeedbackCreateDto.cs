using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Transaction;

public record FeedbackCreateDto(
    [Required]
    [Range(1, 5)]
    int Rating,

    [Required]
    Guid FromUserId,

    [StringLength(1000)]
    string? Comment = null,

    Guid? OrderId = null
);
