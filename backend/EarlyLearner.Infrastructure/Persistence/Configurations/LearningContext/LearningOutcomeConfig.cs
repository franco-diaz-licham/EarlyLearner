using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningContext;

public sealed class LearningOutcomeConfig : IEntityTypeConfiguration<LearningOutcome>
{
    public void Configure(EntityTypeBuilder<LearningOutcome> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(LearningOutcome)));

        builder.HasKey(outcome => outcome.Id);

        builder.Property(outcome => outcome.Id)
            .HasConversion(id => id.Value, value => new LearningOutcomeId(value))
            .ValueGeneratedNever();

        builder.Property(outcome => outcome.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.Property(log => log.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.Property(outcome => outcome.Code).HasMaxLength(80).IsRequired();
        builder.Property(outcome => outcome.Name).HasMaxLength(180).IsRequired();
        builder.Property(outcome => outcome.Description).HasMaxLength(1200).IsRequired();
        builder.Property(outcome => outcome.Category).HasMaxLength(120).IsRequired();
        builder.Property(outcome => outcome.SortOrder).IsRequired();
        builder.Property(outcome => outcome.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(outcome => new { outcome.HouseholdId, outcome.Code }).IsUnique();
        builder.HasIndex(outcome => new { outcome.HouseholdId, outcome.Category, outcome.SortOrder });
        builder.Property(outcome => outcome.CreatedOn).IsRequired();
        builder.Property(outcome => outcome.UpdatedOn).IsRequired(false);
        builder.Ignore(outcome => outcome.DomainEvents);
    }
}
