using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessOutcomeConfig : IEntityTypeConfiguration<ReadinessOutcome>
{
    public void Configure(EntityTypeBuilder<ReadinessOutcome> builder)
    {
        builder.ToTable("readiness_outcomes");

        builder.HasKey(outcome => outcome.Id);

        builder.Property(outcome => outcome.Id)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(outcome => outcome.Code).HasMaxLength(80).IsRequired().HasColumnName("code");
        builder.Property(outcome => outcome.Name).HasMaxLength(180).IsRequired().HasColumnName("name");
        builder.Property(outcome => outcome.Description).HasMaxLength(1200).IsRequired().HasColumnName("description");
        builder.Property(outcome => outcome.Category).HasMaxLength(120).IsRequired().HasColumnName("category");
        builder.Property(outcome => outcome.SortOrder).IsRequired().HasColumnName("sort_order");
        builder.Property(outcome => outcome.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("status");

        builder.HasIndex(outcome => outcome.Code).IsUnique();
        builder.HasIndex(outcome => new { outcome.Category, outcome.SortOrder });
        builder.Ignore(outcome => outcome.DomainEvents);
    }
}
