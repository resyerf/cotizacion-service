namespace Cotizacion.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}
