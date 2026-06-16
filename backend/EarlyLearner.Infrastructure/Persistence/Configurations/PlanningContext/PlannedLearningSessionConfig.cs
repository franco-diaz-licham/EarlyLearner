using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using EarlyLearner.Shared;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.PlanningContext;

public sealed class PlannedLearningSessionConfig : IEntityTypeConfiguration<PlannedLearningSession>
{
    public void Configure(EntityTypeBuilder<PlannedLearningSession> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(PlannedLearningSession)));

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Id)
            .HasConversion(id => id.Value, value => new PlannedLearningSessionId(value))
            .ValueGeneratedNever();

        builder.Property(session => session.LearningPlanId)
            .HasConversion(id => id.Value, value => new LearningPlanId(value))
            .IsRequired();

        builder.HasOne(session => session.LearningPlan)
            .WithMany(plan => plan.Sessions)
            .HasForeignKey(session => session.LearningPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(session => session.PlannedDate)
            .IsRequired();

        builder.Property(session => session.Title)
            .HasMaxLength(220)
            .IsRequired();

        builder.Property(session => session.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property<List<GoalId>>("_goalIds")
            .HasConversion(
                goalIds => JsonSerializer.Serialize(goalIds.Select(goalId => goalId.Value).ToArray(), JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<Guid[]>(value, JsonSerializerOptions.Default)!
                    .Select(goalId => new GoalId(goalId))
                    .ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<GoalId>>(
                (left, right) => left != null && right != null && left.SequenceEqual(right),
                goalIds => goalIds.Aggregate(0, (hash, goalId) => HashCode.Combine(hash, goalId.GetHashCode())),
                goalIds => goalIds.ToList()));

        builder.HasMany(session => session.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<PlannedLearningSessionReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.PlannedLearningSession)
                    .WithMany()
                    .HasForeignKey(join => join.PlannedLearningSessionId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.PlannedLearningSessionId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(PlannedLearningSessionReadinessOutcome)));
                    join.Property(item => item.PlannedLearningSessionId)
                        .HasConversion(id => id.Value, value => new PlannedLearningSessionId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.Navigation(session => session.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(session => new { session.LearningPlanId, session.PlannedDate });
        builder.Ignore(session => session.DomainEvents);
    }

    private sealed class PlannedLearningSessionReadinessOutcome
    {
        public PlannedLearningSessionId PlannedLearningSessionId { get; set; }
        public PlannedLearningSession PlannedLearningSession { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }
}
