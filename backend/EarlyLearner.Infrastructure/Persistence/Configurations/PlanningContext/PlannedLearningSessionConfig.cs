using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        builder.HasMany(session => session.Goals)
            .WithMany()
            .UsingEntity<PlannedLearningSessionGoal>(
                right => right
                    .HasOne(join => join.Goal)
                    .WithMany()
                    .HasForeignKey(join => join.GoalId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.PlannedLearningSession)
                    .WithMany()
                    .HasForeignKey(join => join.PlannedLearningSessionId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.PlannedLearningSessionId, item.GoalId });
                    join.HasIndex(item => item.GoalId);
                    join.ToTable(StringHelpers.Pluralise(nameof(PlannedLearningSessionGoal)));
                    join.Property(item => item.PlannedLearningSessionId)
                        .HasConversion(id => id.Value, value => new PlannedLearningSessionId(value));
                    join.Property(item => item.GoalId)
                        .HasConversion(id => id.Value, value => new GoalId(value));
                });

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

        builder.Navigation(session => session.Goals).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(session => session.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(session => new { session.LearningPlanId, session.PlannedDate });
        builder.Property(session => session.CreatedOn).IsRequired();
        builder.Property(session => session.UpdatedOn).IsRequired(false);
        builder.Ignore(session => session.DomainEvents);
    }

    private sealed class PlannedLearningSessionReadinessOutcome
    {
        public PlannedLearningSessionId PlannedLearningSessionId { get; set; }
        public PlannedLearningSession PlannedLearningSession { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }

    private sealed class PlannedLearningSessionGoal
    {
        public PlannedLearningSessionId PlannedLearningSessionId { get; set; }
        public PlannedLearningSession PlannedLearningSession { get; set; } = null!;
        public GoalId GoalId { get; set; }
        public Goal Goal { get; set; } = null!;
    }
}
