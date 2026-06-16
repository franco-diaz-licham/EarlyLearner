using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class SuggestedNextStepConfig : IEntityTypeConfiguration<SuggestedNextStep>
{
    public void Configure(EntityTypeBuilder<SuggestedNextStep> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(SuggestedNextStep)));

        builder.HasKey(step => step.Id);

        builder.Property(step => step.Id)
            .HasConversion(id => id.Value, value => new SuggestedNextStepId(value))
            .ValueGeneratedNever();

        builder.Property(step => step.ReadinessProfileId)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .IsRequired();

        builder.HasOne(step => step.ReadinessProfile)
            .WithMany(profile => profile.SuggestedNextSteps)
            .HasForeignKey(step => step.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(step => step.ReadinessOutcome)
            .WithMany()
            .HasForeignKey(step => step.ReadinessOutcomeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(step => step.ReadinessOutcomeId)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .IsRequired();

        builder.Property(step => step.Text)
            .HasMaxLength(800)
            .IsRequired();

        builder.Ignore(step => step.DomainEvents);
    }
}
