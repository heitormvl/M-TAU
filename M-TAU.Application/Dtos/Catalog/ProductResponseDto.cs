using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Dtos.Catalog;

public record ProductResponseDto(
    Guid Id,
    string Title,
    decimal Price,
    ProductStatus Status,
    Guid SellerId,
    string? Description = null,
    TechnicalSpecResponseDto? TechnicalSpec = null,
    IReadOnlyCollection<PhotoResponseDto>? Photos = null
);
