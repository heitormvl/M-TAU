using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Catalog;

public record ProductCreateDto(
    [Required]
    [StringLength(120, MinimumLength = 3)]
    string Title,

    [Required]
    [Range(0.01, double.MaxValue)]
    decimal Price,

    [Required]
    Guid SellerId,

    [StringLength(1000)]
    string? Description = null,

    TechnicalSpecCreateDto? TechnicalSpec = null
);
