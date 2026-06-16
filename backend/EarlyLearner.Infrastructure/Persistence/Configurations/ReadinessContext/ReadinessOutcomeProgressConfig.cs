using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessOutcomeProgressConfig : IEntityTypeConfiguration<ReadinessOutcomeProgress>
{
    public void Configure(EntityTypeBuilder<ReadinessOutcomeProgress> builder)
    {
        builder.ToTable("readiness_outcome_progress");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property<ReadinessProfileId>("ReadinessProfileId")
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
            .HasColumnName("readiness_profile_id")
            .IsRequired();

        builder.HasOne<ReadinessProfile>()
            .WithMany(profile => profile.OutcomeProgress)
            .HasForeignKey("ReadinessProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(progress => progress.ReadinessOutcome)
            .WithMany()
            .HasForeignKey("ReadinessOutcomeId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property<ReadinessOutcomeId>("ReadinessOutcomeId")
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .HasColumnName("readiness_outcome_id")
            .IsRequired();

        builder.Property(progress => progress.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("status");

        builder.HasMany(progress => progress.Evidence)
            .WithOne()
            .HasForeignKey("ReadinessOutcomeProgressId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(progress => progress.Evidence).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex("ReadinessProfileId", "ReadinessOutcomeId").IsUnique();
    }
}
