using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Dtos.Catalog;

public record TechnicalSpecCreateDto(
    [Required]
    DisabilityCategory Category,

    [StringLength(500)]
    string? Measures = null,

    [Range(0.0, double.MaxValue)]
    double? WeightCapacity = null,

    [StringLength(200)]
    string? UsageTime = null
);
