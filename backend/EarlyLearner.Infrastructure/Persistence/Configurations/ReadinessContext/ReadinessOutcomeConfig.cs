using EarlyLearner.Domain.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class ReadinessOutcomeConfig : IEntityTypeConfiguration<ReadinessOutcome>
{
    public void Configure(EntityTypeBuilder<ReadinessOutcome> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadinessOutcome)));

        builder.HasKey(outcome => outcome.Id);

        builder.Property(outcome => outcome.Id)
            .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value))
            .ValueGeneratedNever();

        builder.Property(outcome => outcome.Code).HasMaxLength(80).IsRequired();
        builder.Property(outcome => outcome.Name).HasMaxLength(180).IsRequired();
        builder.Property(outcome => outcome.Description).HasMaxLength(1200).IsRequired();
        builder.Property(outcome => outcome.Category).HasMaxLength(120).IsRequired();
        builder.Property(outcome => outcome.SortOrder).IsRequired();
        builder.Property(outcome => outcome.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(outcome => outcome.Code).IsUnique();
        builder.HasIndex(outcome => new { outcome.Category, outcome.SortOrder });
        builder.Property(outcome => outcome.CreatedOn).IsRequired();
        builder.Property(outcome => outcome.UpdatedOn).IsRequired(false);
        builder.Ignore(outcome => outcome.DomainEvents);
    }
}
