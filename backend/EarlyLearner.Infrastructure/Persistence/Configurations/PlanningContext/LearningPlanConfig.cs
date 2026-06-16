using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.PlanningContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.PlanningContext;

public sealed class LearningPlanConfig : IEntityTypeConfiguration<LearningPlan>
{
    public void Configure(EntityTypeBuilder<LearningPlan> builder)
    {
        builder.ToTable("learning_plans");

        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .HasConversion(id => id.Value, value => new LearningPlanId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(plan => plan.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(plan => plan.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(plan => plan.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .HasColumnName("child_id")
            .IsRequired();

        builder.HasOne<Child>()
            .WithMany()
            .HasForeignKey(plan => plan.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(plan => plan.Focus)
            .HasMaxLength(260)
            .IsRequired()
            .HasColumnName("focus");

        builder.OwnsOne(
            plan => plan.Period,
            period =>
            {
                period.Property(range => range.StartDate).HasColumnName("start_date").IsRequired();
                period.Property(range => range.EndDate).HasColumnName("end_date").IsRequired();
            });

        builder.HasMany(plan => plan.Sessions)
            .WithOne()
            .HasForeignKey("LearningPlanId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(plan => plan.Sessions).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(plan => new { plan.HouseholdId, plan.ChildId });
        builder.Ignore(plan => plan.DomainEvents);
    }
}
