using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class ObservationConfig : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.ToTable("observations");

        builder.HasKey(observation => observation.Id);

        builder.Property(observation => observation.Id)
            .HasConversion(id => id.Value, value => new ObservationId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(observation => observation.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(observation => observation.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(observation => observation.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .HasColumnName("child_id")
            .IsRequired();

        builder.HasOne<Child>()
            .WithMany()
            .HasForeignKey(observation => observation.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(observation => observation.ObservedOn).HasColumnName("observed_on").IsRequired();
        builder.Property(observation => observation.Note).HasMaxLength(2400).IsRequired().HasColumnName("note");

        builder.HasMany(observation => observation.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "observation_readiness_outcomes",
                right => right.HasOne<ReadinessOutcome>()
                    .WithMany()
                    .HasForeignKey("readiness_outcome_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<Observation>()
                    .WithMany()
                    .HasForeignKey("observation_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("observation_readiness_outcomes");
                    join.HasKey("observation_id", "readiness_outcome_id");
                });

        builder.HasMany(observation => observation.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "observation_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<Observation>()
                    .WithMany()
                    .HasForeignKey("observation_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("observation_stored_files");
                    join.HasKey("observation_id", "stored_file_id");
                });

        builder.Navigation(observation => observation.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(observation => observation.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(observation => new { observation.HouseholdId, observation.ChildId, observation.ObservedOn });
        builder.Ignore(observation => observation.DomainEvents);
    }
}
