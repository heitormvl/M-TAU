namespace M_TAU.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (current, component) => HashCode.Combine(current, component));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => EqualityComparer<ValueObject>.Default.Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);
}
