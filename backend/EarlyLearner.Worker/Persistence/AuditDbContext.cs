using Microsoft.EntityFrameworkCore;
using MassTransit;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Worker.Persistence;

public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditTrailEntry> AuditTrailEntries => Set<AuditTrailEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditTrailEntry>(builder => {
            builder.ToTable(StringHelpers.Pluralise(nameof(AuditTrailEntry)));
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedNever();
            builder.Property(entry => entry.HouseholdId).IsRequired();
            builder.Property(entry => entry.Action).HasMaxLength(120).IsRequired();
            builder.Property(entry => entry.Summary).HasMaxLength(300).IsRequired();
            builder.Property(entry => entry.Details).HasMaxLength(1200).IsRequired(false);
            builder.Property(entry => entry.ActionedAt).IsRequired();
            builder.Property(entry => entry.RecordedAt).IsRequired();
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        base.OnModelCreating(modelBuilder);
    }
}
