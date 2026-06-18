namespace EarlyLearner.Domain.CoreContext;

/// <summary>
/// Base type for domain entities that have identity and can raise domain events.
/// Aggregate roots expose their state through behaviour and collect events for
/// application/infrastructure dispatch after persistence.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    public void SetCreatedOn()
    {
        CreatedOn = DateTime.UtcNow;
    }

    public void SetUpdatedOn()
    {
        UpdatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Domain events raised by this entity during the current unit of work.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Entities that has a primary Id.
/// </summary>
public abstract class Entity<TId> : Entity where TId : notnull
{
    public TId Id { get; protected set; } = default!;
}
