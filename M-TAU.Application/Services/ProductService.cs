using AutoMapper;
using M_TAU.Application.Dtos.Catalog;
using M_TAU.Application.Dtos.Common;
using M_TAU.Domain.Catalog;
using M_TAU.Domain.Repositories;

namespace M_TAU.Application.Services;

public sealed class ProductService(IProductRepository productRepository, IMapper mapper) : IProductService
{
    public async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{id}' not found.");
        return mapper.Map<ProductResponseDto>(product);
    }

    public async Task<PaginatedResult<ProductResponseDto>> GetAllAsync(ProductFilterDto filter, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Product> products;
        if (filter.SellerId.HasValue)
            products = await productRepository.ListBySellerAsync(filter.SellerId.Value, cancellationToken);
        else if (filter.Category.HasValue)
            products = await productRepository.ListByCategoryAsync(filter.Category.Value, cancellationToken);
        else
            products = await productRepository.ListAsync(cancellationToken);

        var query = products.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(p => p.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);
        if (filter.SellerId.HasValue && filter.Category.HasValue)
            query = query.Where(p => p.TechnicalSpec?.Category == filter.Category.Value);

        var filtered = query.ToList();
        var totalCount = filtered.Count;

        var pageNumber = filter.PageNumber is > 0 ? filter.PageNumber.Value : 1;
        var pageSize = filter.PageSize is > 0 ? filter.PageSize.Value : totalCount;
        var paged = pageSize <= 0
            ? filtered
            : filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var mapped = mapper.Map<IReadOnlyCollection<ProductResponseDto>>(paged);
        return PaginatedResult<ProductResponseDto>.Create(mapped, totalCount, pageNumber, pageSize);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var product = new Product(Guid.NewGuid(), createDto.Title, createDto.Price, createDto.SellerId);
        product.SetDescription(createDto.Description);

        if (createDto.TechnicalSpec is not null)
        {
            var spec = new TechnicalSpec(Guid.NewGuid(), createDto.TechnicalSpec.Category);
            if (createDto.TechnicalSpec.Measures is not null)
                spec.SetMeasures(createDto.TechnicalSpec.Measures);
            if (createDto.TechnicalSpec.WeightCapacity.HasValue)
                spec.SetWeightCapacity(createDto.TechnicalSpec.WeightCapacity.Value);
            if (createDto.TechnicalSpec.UsageTime is not null)
                spec.SetUsageTime(createDto.TechnicalSpec.UsageTime);
            product.SetTechnicalSpec(spec);
        }

        await productRepository.AddAsync(product, cancellationToken);
        return mapper.Map<ProductResponseDto>(product);
    }

    public async Task UpdateAsync(Guid id, ProductCreateDto updateDto, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{id}' not found.");
        product.SetTitle(updateDto.Title);
        product.SetPrice(updateDto.Price);
        product.SetDescription(updateDto.Description);
        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, ProductStatus status, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{id}' not found.");
        product.UpdateStatus(status);
        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task<ProductResponseDto> AddPhotoAsync(Guid productId, PhotoCreateDto photoDto, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{productId}' not found.");
        var photo = new Photo(Guid.NewGuid(), photoDto.Url, photoDto.IsMain);
        product.AddPhoto(photo);
        await productRepository.UpdateAsync(product, cancellationToken);
        return mapper.Map<ProductResponseDto>(product);
    }

    public async Task RemovePhotoAsync(Guid productId, Guid photoId, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{productId}' not found.");
        product.RemovePhoto(photoId);
        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{id}' not found.");
        await productRepository.DeleteAsync(product, cancellationToken);
    }
}
