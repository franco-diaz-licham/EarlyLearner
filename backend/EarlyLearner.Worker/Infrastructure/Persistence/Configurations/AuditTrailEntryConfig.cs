using EarlyLearner.Worker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Worker.Infrastructure.Persistence.Configurations;

public sealed class AuditTrailEntryConfig : IEntityTypeConfiguration<AuditTrailEntry>
{
    public void Configure(EntityTypeBuilder<AuditTrailEntry> builder)
    {
        builder.ToTable("audit_trail_entries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id).ValueGeneratedNever();
        builder.Property(entry => entry.HouseholdId).IsRequired();
        builder.Property(entry => entry.Action).HasMaxLength(120).IsRequired();
        builder.Property(entry => entry.Summary).HasMaxLength(300).IsRequired();
        builder.Property(entry => entry.Details).HasMaxLength(1200).IsRequired(false);
        builder.Property(entry => entry.ActionedAt).IsRequired();
        builder.Property(entry => entry.RecordedAt).IsRequired();

        builder.HasIndex(entry => new { entry.HouseholdId, entry.ActionedAt });
    }
}
