using EarlyLearner.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.AuditContext;

public sealed class AuditTrailReadModelConfig : IEntityTypeConfiguration<AuditTrailReadModel>
{
    public void Configure(EntityTypeBuilder<AuditTrailReadModel> builder)
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
    }
}
