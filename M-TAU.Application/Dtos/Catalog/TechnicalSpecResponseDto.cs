using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Dtos.Catalog;

public record TechnicalSpecResponseDto(
    Guid Id,
    DisabilityCategory Category,
    double WeightCapacity,
    string? Measures = null,
    string? UsageTime = null
);
