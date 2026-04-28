using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Catalog;

public class TechnicalSpec : EntityBase<Guid>
{
    [Required]
    public DisabilityCategory Category { get; private set; }

    [StringLength(500)]
    public string? Measures { get; private set; }

    [Range(0.0, double.MaxValue)]
    public double WeightCapacity { get; private set; }

    [StringLength(200)]
    public string? UsageTime { get; private set; }

    protected TechnicalSpec() { }

    public TechnicalSpec(Guid id, DisabilityCategory category)
        : base(id)
    {
        Category = category;
        WeightCapacity = 0.0;
    }

    public void SetMeasures(string? measures)
    {
        Measures = string.IsNullOrWhiteSpace(measures) ? null : measures.Trim();
    }

    public void SetWeightCapacity(double capacity)
    {
        if (capacity < 0)
            throw new ArgumentException("Weight capacity must be non-negative.", nameof(capacity));

        WeightCapacity = capacity;
    }

    public void SetUsageTime(string? usageTime)
    {
        UsageTime = string.IsNullOrWhiteSpace(usageTime) ? null : usageTime.Trim();
    }
}
