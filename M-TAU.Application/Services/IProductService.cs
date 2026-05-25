using M_TAU.Application.Dtos.Catalog;
using M_TAU.Application.Dtos.Common;
using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Services;

public interface IProductService
{
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedResult<ProductResponseDto>> GetAllAsync(ProductFilterDto filter, CancellationToken cancellationToken = default);

    Task<ProductResponseDto> CreateAsync(ProductCreateDto createDto, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, ProductCreateDto updateDto, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Guid id, ProductStatus status, CancellationToken cancellationToken = default);

    Task<ProductResponseDto> AddPhotoAsync(Guid productId, PhotoCreateDto photoDto, CancellationToken cancellationToken = default);

    Task RemovePhotoAsync(Guid productId, Guid photoId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
