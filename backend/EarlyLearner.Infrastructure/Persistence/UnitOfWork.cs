using EarlyLearner.Application.Common;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EarlyLearner.Infrastructure.Persistence;

public sealed class UnitOfWork(DatabaseContext db, IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        RaiseEntityTraceEvents();

        var entitiesWithEvents = db.ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        // MassTransit captures integration publishes in the EF Bus Outbox here.
        // SaveChanges then commits aggregate changes and outbox messages together.
        if (domainEvents.Count > 0) await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        var result = await db.SaveChangesAsync(cancellationToken);
        foreach (var entity in entitiesWithEvents) entity.ClearDomainEvents();
        return result;
    }

    private void RaiseEntityTraceEvents()
    {
        var now = DateTimeOffset.UtcNow;
        var entries = db.ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries) {
            var householdId = TryGetHouseholdId(entry.Entity);
            if (householdId is null) continue;

            entry.Entity.RaiseTraceEvent(
                entityName: entry.Entity.GetType().Name,
                entityId: TryGetEntityId(entry.Entity) ?? "unknown",
                action: $"Entity{entry.State}",
                householdId: householdId.Value,
                occurredAt: now);
        }
    }

    private static Guid? TryGetHouseholdId(Entity entity)
    {
        if (entity is Household household) return household.Id.Value;

        var householdIdProperty = entity.GetType().GetProperty("HouseholdId");
        var householdId = householdIdProperty?.GetValue(entity);
        var valueProperty = householdId?.GetType().GetProperty("Value");
        return valueProperty?.GetValue(householdId) is Guid value ? value : null;
    }

    private static string? TryGetEntityId(Entity entity)
    {
        var id = entity.GetType().GetProperty("Id")?.GetValue(entity);
        if (id is null) return null;

        var value = id.GetType().GetProperty("Value")?.GetValue(id);
        return value?.ToString() ?? id.ToString();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        transaction ??= await db.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is null) return;

        await transaction.CommitAsync(cancellationToken);
        await transaction.DisposeAsync();
        transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is null) return;

        await transaction.RollbackAsync(cancellationToken);
        await transaction.DisposeAsync();
        transaction = null;
    }
}
