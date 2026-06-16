using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
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

        builder.Property<LearningPlanId>("LearningPlanId")
            .HasConversion(id => id.Value, value => new LearningPlanId(value))
            .IsRequired();

        builder.HasOne<LearningPlan>()
            .WithMany(plan => plan.Sessions)
            .HasForeignKey("LearningPlanId")
            .OnDelete(DeleteBehavior.Cascade);

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
            .UsingEntity<Dictionary<string, object>>(
                "planned_session_readiness_outcomes",
                right => right.HasOne<ReadinessOutcome>()
                    .WithMany()
                    .HasForeignKey("readiness_outcome_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<PlannedLearningSession>()
                    .WithMany()
                    .HasForeignKey("planned_learning_session_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.ToTable("planned_session_readiness_outcomes");
                    join.HasKey("planned_learning_session_id", "readiness_outcome_id");
                });

        builder.Navigation(session => session.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex("LearningPlanId", nameof(PlannedLearningSession.PlannedDate));
        builder.Ignore(session => session.DomainEvents);
    }
}
