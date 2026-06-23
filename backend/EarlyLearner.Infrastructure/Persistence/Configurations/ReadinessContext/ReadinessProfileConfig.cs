using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessProfileConfig : IEntityTypeConfiguration<ReadinessProfile>
{
    public void Configure(EntityTypeBuilder<ReadinessProfile> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadinessProfile)));

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .ValueGeneratedNever();

        builder.Property(profile => profile.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(profile => profile.Household)
            .WithMany()
            .HasForeignKey(profile => profile.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(profile => profile.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne(profile => profile.Child)
            .WithMany()
            .HasForeignKey(profile => profile.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(profile => profile.OutcomeProgress)
            .WithOne(progress => progress.ReadinessProfile)
            .HasForeignKey(progress => progress.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.SuggestedNextSteps)
            .WithOne(step => step.ReadinessProfile)
            .HasForeignKey(step => step.ReadinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(profile => profile.OutcomeProgress).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(profile => profile.SuggestedNextSteps).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(profile => new { profile.HouseholdId, profile.ChildId }).IsUnique();
        builder.Property(profile => profile.CreatedOn).IsRequired();
        builder.Property(profile => profile.UpdatedOn).IsRequired(false);
        builder.Ignore(profile => profile.DomainEvents);
    }
}
