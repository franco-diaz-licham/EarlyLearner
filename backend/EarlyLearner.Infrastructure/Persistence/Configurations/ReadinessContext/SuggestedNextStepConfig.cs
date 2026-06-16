using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class SuggestedNextStepConfig : IEntityTypeConfiguration<SuggestedNextStep>
{
    public void Configure(EntityTypeBuilder<SuggestedNextStep> builder)
    {
        builder.ToTable("suggested_next_steps");

        builder.HasKey(step => step.Id);

        builder.Property(step => step.Id)
            .HasConversion(id => id.Value, value => new SuggestedNextStepId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<ReadinessProfileId>("ReadinessProfileId")
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .HasColumnName("readiness_profile_id")
            .IsRequired();

        builder.HasOne<ReadinessProfile>()
            .WithMany(profile => profile.SuggestedNextSteps)
            .HasForeignKey("ReadinessProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(step => step.ReadinessOutcome)
            .WithMany()
            .HasForeignKey("ReadinessOutcomeId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property<ReadinessOutcomeId>("ReadinessOutcomeId")
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .HasColumnName("readiness_outcome_id")
            .IsRequired();

        builder.Property(step => step.Text)
            .HasMaxLength(800)
            .IsRequired()
            .HasColumnName("text");

        builder.Ignore(step => step.DomainEvents);
    }
}
