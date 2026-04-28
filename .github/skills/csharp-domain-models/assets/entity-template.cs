using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Entities;

public class SampleEntity : EntityBase<Guid>
{
    [Required]
    [StringLength(120)]
    public string Name { get; private set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; private set; }

    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private readonly List<string> _tags = [];

    protected SampleEntity()
    {
    }

    public SampleEntity(Guid id, string name, string? description = null)
        : base(id)
    {
        SetName(name);
        Description = description;
    }

    public void SetName(string name)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name is required.", nameof(name))
            : name.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}