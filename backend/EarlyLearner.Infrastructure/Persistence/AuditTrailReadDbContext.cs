using EarlyLearner.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence;

public sealed class AuditTrailReadDbContext(DbContextOptions<AuditTrailReadDbContext> options) : DbContext(options)
{
    public DbSet<AuditTrailReadModel> AuditTrailEntries => Set<AuditTrailReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditTrailReadModel>(builder => {
            builder.ToTable("audit_trail_entries", DatabaseSchemas.Worker);
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedNever();
            builder.Property(entry => entry.HouseholdId).IsRequired();
            builder.Property(entry => entry.Action).HasMaxLength(120).IsRequired();
            builder.Property(entry => entry.Summary).HasMaxLength(300).IsRequired();
            builder.Property(entry => entry.Details).HasMaxLength(1200).IsRequired(false);
            builder.Property(entry => entry.ActionedAt).IsRequired();
            builder.Property(entry => entry.RecordedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
