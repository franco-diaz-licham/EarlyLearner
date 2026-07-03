using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessEvidenceConfig : IEntityTypeConfiguration<ReadinessEvidence>
{
    public void Configure(EntityTypeBuilder<ReadinessEvidence> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadinessEvidence)));

        builder.HasKey(evidence => evidence.Id);

        builder.Property(evidence => evidence.Id)
            .HasConversion(id => id.Value, value => new ReadinessEvidenceId(value))
            .ValueGeneratedNever();

        builder.Property(evidence => evidence.ReadinessProfileId)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .IsRequired();

        builder.HasOne(evidence => evidence.ReadinessProfile)
            .WithMany(profile => profile.Evidence)
            .HasForeignKey(evidence => evidence.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(evidence => evidence.ReadinessOutcome)
            .WithMany()
            .HasForeignKey(evidence => evidence.ReadinessOutcomeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(evidence => evidence.ReadinessOutcomeId)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .IsRequired();

        builder.Property(evidence => evidence.SourceType)
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(evidence => evidence.EvidenceRecordId).IsRequired();
        builder.Property(evidence => evidence.ObservedOn).IsRequired();
        builder.Property(evidence => evidence.Summary).HasMaxLength(800).IsRequired();
        builder.Property(evidence => evidence.CreatedOn).IsRequired();
        builder.Property(evidence => evidence.UpdatedOn).IsRequired(false);
        builder.Ignore(evidence => evidence.DomainEvents);
    }
}
