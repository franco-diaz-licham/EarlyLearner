using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.PlanningContext;

public sealed class GoalConfig : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.ToTable("goals");

        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.Id)
            .HasConversion(id => id.Value, value => new GoalId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(goal => goal.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(goal => goal.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(goal => goal.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .HasColumnName("child_id")
            .IsRequired();

        builder.HasOne<Child>()
            .WithMany()
            .HasForeignKey(goal => goal.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(goal => goal.Title)
            .HasMaxLength(220)
            .IsRequired()
            .HasColumnName("title");

        builder.Property(goal => goal.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("type");

        builder.Property(goal => goal.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("status");

        builder.OwnsOne(
            goal => goal.Timeframe,
            timeframe =>
            {
                timeframe.Property(range => range.StartDate).HasColumnName("start_date").IsRequired();
                timeframe.Property(range => range.EndDate).HasColumnName("end_date").IsRequired();
            });

        builder.HasMany(goal => goal.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "goal_readiness_outcomes",
                right => right.HasOne<ReadinessOutcome>()
                    .WithMany()
                    .HasForeignKey("readiness_outcome_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<Goal>()
                    .WithMany()
                    .HasForeignKey("goal_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("goal_readiness_outcomes");
                    join.HasKey("goal_id", "readiness_outcome_id");
                });

        builder.Navigation(goal => goal.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(goal => new { goal.HouseholdId, goal.ChildId, goal.Status });
        builder.Ignore(goal => goal.DomainEvents);
    }
}
