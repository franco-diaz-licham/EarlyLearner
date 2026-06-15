namespace EarlyLearner.Domain.CoreContext;

/// <summary>
/// Base type for domain entities that have identity and can raise domain events.
/// Aggregate roots expose their state through behaviour and collect events for
/// application/infrastructure dispatch after persistence.
/// </summary>
public abstract class Entity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// The stable domain identity for this entity.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    /// Domain events raised by this entity during the current unit of work.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
