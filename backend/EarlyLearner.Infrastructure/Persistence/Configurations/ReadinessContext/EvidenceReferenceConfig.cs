using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class EvidenceReferenceConfig : IEntityTypeConfiguration<EvidenceReference>
{
    public void Configure(EntityTypeBuilder<EvidenceReference> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(EvidenceReference)));

        builder.HasKey(evidence => evidence.Id);

        builder.Property(evidence => evidence.Id)
            .HasConversion(id => id.Value, value => new EvidenceReferenceId(value))
            .ValueGeneratedNever();

        builder.Property<int>("ReadinessOutcomeProgressId")
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
            .IsRequired();

        builder.Property(evidence => evidence.SourceType)
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(evidence => evidence.EvidenceRecordId).IsRequired();
        builder.Property(evidence => evidence.ObservedOn).IsRequired();
        builder.Property(evidence => evidence.Summary).HasMaxLength(800).IsRequired();
        builder.Ignore(evidence => evidence.DomainEvents);
    }
}
