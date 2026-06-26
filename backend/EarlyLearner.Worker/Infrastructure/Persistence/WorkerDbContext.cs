using EarlyLearner.Worker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Worker.Infrastructure.Persistence;

public sealed class WorkerDbContext(DbContextOptions<WorkerDbContext> options) : DbContext(options)
{
    public DbSet<AuditTrailEntry> AuditTrailEntries => Set<AuditTrailEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(WorkerDatabaseSchemas.Worker);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
