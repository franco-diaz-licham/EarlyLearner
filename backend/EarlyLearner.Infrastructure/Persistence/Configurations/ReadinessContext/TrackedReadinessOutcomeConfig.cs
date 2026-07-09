using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class TrackedReadinessOutcomeConfig : IEntityTypeConfiguration<TrackedReadinessOutcome>
{
    public void Configure(EntityTypeBuilder<TrackedReadinessOutcome> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(TrackedReadinessOutcome)));

        builder.HasKey(trackedOutcome => trackedOutcome.Id);
        builder.Property(trackedOutcome => trackedOutcome.Id).ValueGeneratedOnAdd();

        builder.Property(trackedOutcome => trackedOutcome.ReadinessProfileId)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .IsRequired();

        builder.HasOne(trackedOutcome => trackedOutcome.ReadinessProfile)
            .WithMany(profile => profile.TrackedOutcomes)
            .HasForeignKey(trackedOutcome => trackedOutcome.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(trackedOutcome => trackedOutcome.ReadinessOutcome)
            .WithMany()
            .HasForeignKey(trackedOutcome => trackedOutcome.ReadinessOutcomeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(trackedOutcome => trackedOutcome.ReadinessOutcomeId)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .IsRequired();

        builder.Property(trackedOutcome => trackedOutcome.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(trackedOutcome => new { trackedOutcome.ReadinessProfileId, trackedOutcome.ReadinessOutcomeId }).IsUnique();
    }
}
