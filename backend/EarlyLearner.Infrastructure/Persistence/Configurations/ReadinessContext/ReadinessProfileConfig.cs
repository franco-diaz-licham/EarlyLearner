using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessProfileConfig : IEntityTypeConfiguration<ReadinessProfile>
{
    public void Configure(EntityTypeBuilder<ReadinessProfile> builder)
    {
        builder.ToTable("readiness_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(profile => profile.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(profile => profile.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(profile => profile.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .HasColumnName("child_id")
            .IsRequired();

        builder.HasOne<Child>()
            .WithMany()
            .HasForeignKey(profile => profile.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(profile => profile.OutcomeProgress)
            .WithOne()
            .HasForeignKey("ReadinessProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.SuggestedNextSteps)
            .WithOne()
            .HasForeignKey("ReadinessProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(profile => profile.OutcomeProgress).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(profile => profile.SuggestedNextSteps).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(profile => new { profile.HouseholdId, profile.ChildId }).IsUnique();
        builder.Ignore(profile => profile.DomainEvents);
    }
}
