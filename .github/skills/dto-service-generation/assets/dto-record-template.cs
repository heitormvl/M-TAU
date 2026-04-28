using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.{Aggregate};

/// <summary>
/// CreateDto for {Entity} – Input DTO for creating a new {Entity} instance.
/// Contains only properties that a client sends when creating.
/// Excludes: Id, CreatedAt, UpdatedAt.
/// </summary>
public record {Entity}CreateDto(
    [Required]
    [StringLength(120, MinimumLength = 3)]
    string PropertyName,
    
    [Range(0.01, double.MaxValue)]
    decimal PropertyPrice,
    
    [StringLength(1000)]
    string? PropertyDescription = null
);

/// <summary>
/// ResponseDto for {Entity} – Output DTO for reading {Entity} data.
/// Contains all properties the client needs to see, including Id and timestamps.
/// </summary>
public record {Entity}ResponseDto(
    [Required]
    Guid Id,
    
    [Required]
    string PropertyName,
    
    [Required]
    decimal PropertyPrice,
    
    string? PropertyDescription,
    
    DateTime CreatedAt,
    
    DateTime? UpdatedAt = null
);

/// <summary>
/// FilterDto for {Entity} – Input DTO for filtering/searching {Entity} records.
/// All properties are optional (with default null).
/// Includes pagination support.
/// </summary>
public record {Entity}FilterDto(
    string? PropertyName = null,
    
    decimal? PropertyPriceMin = null,
    
    decimal? PropertyPriceMax = null,
    
    [Range(1, int.MaxValue)]
    int? PageNumber = null,
    
    [Range(1, 100)]
    int? PageSize = null
);
