using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Dtos.Catalog;

public record ProductFilterDto(
    string? Title = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    ProductStatus? Status = null,
    Guid? SellerId = null,
    DisabilityCategory? Category = null,
    int? PageNumber = null,
    int? PageSize = null
);
