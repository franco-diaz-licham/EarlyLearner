using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class ObservationConfig : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(Observation)));

        builder.HasKey(observation => observation.Id);

        builder.Property(observation => observation.Id)
            .HasConversion(id => id.Value, value => new ObservationId(value))
            .ValueGeneratedNever();

        builder.Property(observation => observation.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(observation => observation.Household)
            .WithMany()
            .HasForeignKey(observation => observation.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(observation => observation.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne(observation => observation.Child)
            .WithMany()
            .HasForeignKey(observation => observation.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(observation => observation.ObservedOn).IsRequired();
        builder.Property(observation => observation.Note).HasMaxLength(2400).IsRequired();

        builder.HasMany(observation => observation.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<ObservationReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.Observation)
                    .WithMany()
                    .HasForeignKey(join => join.ObservationId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.ObservationId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(ObservationReadinessOutcome)));
                    join.Property(item => item.ObservationId)
                        .HasConversion(id => id.Value, value => new ObservationId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.HasMany(observation => observation.StoredFiles)
            .WithMany()
            .UsingEntity<ObservationStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.Observation)
                    .WithMany()
                    .HasForeignKey(join => join.ObservationId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.ObservationId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(ObservationStoredFile)));
                    join.Property(item => item.ObservationId)
                        .HasConversion(id => id.Value, value => new ObservationId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(observation => observation.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(observation => observation.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(observation => new { observation.HouseholdId, observation.ChildId, observation.ObservedOn });
        builder.Ignore(observation => observation.DomainEvents);
    }

    private sealed class ObservationReadinessOutcome
    {
        public ObservationId ObservationId { get; set; }
        public Observation Observation { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }

    private sealed class ObservationStoredFile
    {
        public ObservationId ObservationId { get; set; }
        public Observation Observation { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
