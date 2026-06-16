using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessOutcomeProgressConfig : IEntityTypeConfiguration<ReadinessOutcomeProgress>
{
    public void Configure(EntityTypeBuilder<ReadinessOutcomeProgress> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadinessOutcomeProgress)));

        builder.HasKey(progress => progress.Id);
        builder.Property(progress => progress.Id).ValueGeneratedOnAdd();

        builder.Property(progress => progress.ReadinessProfileId)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .IsRequired();

        builder.HasOne(progress => progress.ReadinessProfile)
            .WithMany(profile => profile.OutcomeProgress)
            .HasForeignKey(progress => progress.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(progress => progress.ReadinessOutcome)
            .WithMany()
            .HasForeignKey(progress => progress.ReadinessOutcomeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(progress => progress.ReadinessOutcomeId)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .IsRequired();

        builder.Property(progress => progress.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasMany(progress => progress.Evidence)
            .WithOne(evidence => evidence.ReadinessOutcomeProgress)
            .HasForeignKey(evidence => evidence.ReadinessOutcomeProgressId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(progress => progress.Evidence).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(progress => new { progress.ReadinessProfileId, progress.ReadinessOutcomeId }).IsUnique();
    }
}
