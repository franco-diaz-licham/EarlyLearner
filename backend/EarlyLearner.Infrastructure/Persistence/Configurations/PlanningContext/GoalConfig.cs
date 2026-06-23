using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.PlanningContext;

public sealed class GoalConfig : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(Goal)));

        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.Id)
            .HasConversion(id => id.Value, value => new GoalId(value))
            .ValueGeneratedNever();

        builder.Property(goal => goal.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(goal => goal.Household)
            .WithMany()
            .HasForeignKey(goal => goal.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(goal => goal.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne(goal => goal.Child)
            .WithMany()
            .HasForeignKey(goal => goal.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(goal => goal.Title)
            .HasMaxLength(220)
            .IsRequired();

        builder.Property(goal => goal.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(goal => goal.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.OwnsOne(
            goal => goal.Timeframe,
            timeframe => {
                timeframe.Property(range => range.StartDate).IsRequired();
                timeframe.Property(range => range.EndDate).IsRequired();
            });

        builder.HasMany(goal => goal.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<GoalReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.Goal)
                    .WithMany()
                    .HasForeignKey(join => join.GoalId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.GoalId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(GoalReadinessOutcome)));
                    join.Property(item => item.GoalId)
                        .HasConversion(id => id.Value, value => new GoalId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.Navigation(goal => goal.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(goal => new { goal.HouseholdId, goal.ChildId, goal.Status });
        builder.Property(goal => goal.CreatedOn).IsRequired();
        builder.Property(goal => goal.UpdatedOn).IsRequired(false);
        builder.Ignore(goal => goal.DomainEvents);
    }

    private sealed class GoalReadinessOutcome
    {
        public GoalId GoalId { get; set; }
        public Goal Goal { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }
}
