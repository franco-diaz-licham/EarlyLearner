using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class EvidenceReferenceConfig : IEntityTypeConfiguration<EvidenceReference>
{
    public void Configure(EntityTypeBuilder<EvidenceReference> builder)
    {
        builder.ToTable("evidence_references");

        builder.HasKey(evidence => evidence.Id);

        builder.Property(evidence => evidence.Id)
            .HasConversion(id => id.Value, value => new EvidenceReferenceId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<int>("ReadinessOutcomeProgressId")
            .HasColumnName("readiness_outcome_progress_id")
            .IsRequired();

        builder.HasOne<ReadinessOutcomeProgress>()
            .WithMany(progress => progress.Evidence)
            .HasForeignKey("ReadinessOutcomeProgressId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(evidence => evidence.ReadinessOutcome)
            .WithMany()
            .HasForeignKey("ReadinessOutcomeId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property<ReadinessOutcomeId>("ReadinessOutcomeId")
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .HasColumnName("readiness_outcome_id")
            .IsRequired();

        builder.Property(evidence => evidence.SourceType)
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired()
            .HasColumnName("source_type");

        builder.Property(evidence => evidence.EvidenceRecordId).HasColumnName("evidence_record_id").IsRequired();
        builder.Property(evidence => evidence.ObservedOn).HasColumnName("observed_on").IsRequired();
        builder.Property(evidence => evidence.Summary).HasMaxLength(800).IsRequired().HasColumnName("summary");
        builder.Ignore(evidence => evidence.DomainEvents);
    }
}
