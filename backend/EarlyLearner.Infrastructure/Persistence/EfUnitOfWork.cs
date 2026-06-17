using EarlyLearner.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace EarlyLearner.Infrastructure.Persistence;

public sealed class EfUnitOfWork(DatabaseContext db) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
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
