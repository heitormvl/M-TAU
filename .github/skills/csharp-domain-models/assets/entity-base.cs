using System.ComponentModel.DataAnnotations;

namespace M_TAU.Domain.Common;

public abstract class EntityBase<TKey> where TKey : notnull
{
    [Required]
    public TKey Id { get; protected set; } = default!;

    protected EntityBase()
    {
    }

    protected EntityBase(TKey id)
    {
        Id = id;
    }
}