using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Catalog;

public class Product : EntityBase<Guid>
{
    [Required]
    [StringLength(120)]
    public string Title { get; private set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; private set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; private set; }

    [Required]
    public ProductStatus Status { get; private set; }

    [Required]
    public Guid SellerId { get; private set; }

    public TechnicalSpec? TechnicalSpec { get; private set; }

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();

    private readonly List<Photo> _photos = [];

    protected Product() { }

    public Product(Guid id, string title, decimal price, Guid sellerId)
        : base(id)
    {
        SetTitle(title);
        SetPrice(price);
        SellerId = sellerId;
        Status = ProductStatus.Active;
    }

    public void SetTitle(string title)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title is required.", nameof(title))
            : title.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(price));

        Price = price;
    }

    public void UpdateStatus(ProductStatus status)
    {
        Status = status;
    }

    public void SetTechnicalSpec(TechnicalSpec? spec)
    {
        TechnicalSpec = spec;
    }

    public void AddPhoto(Photo photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));

        _photos.Add(photo);
    }

    public void RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null)
            _photos.Remove(photo);
    }
}
