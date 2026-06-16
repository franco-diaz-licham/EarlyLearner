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

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property<ReadinessProfileId>("ReadinessProfileId")
            .HasConversion(id => id.Value, value => new ReadinessProfileId(value))
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
            .IsRequired();

        builder.Property(progress => progress.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasMany(progress => progress.Evidence)
            .WithOne()
            .HasForeignKey("ReadinessOutcomeProgressId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(progress => progress.Evidence).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex("ReadinessProfileId", "ReadinessOutcomeId").IsUnique();
    }
}
