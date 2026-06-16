using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class CompletedActivityConfig : IEntityTypeConfiguration<CompletedActivity>
{
    public void Configure(EntityTypeBuilder<CompletedActivity> builder)
    {
        builder.ToTable("completed_activities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.Id)
            .HasConversion(id => id.Value, value => new CompletedActivityId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<DailyLogId>("DailyLogId")
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .HasColumnName("daily_log_id")
            .IsRequired();

        builder.HasOne<DailyLog>()
            .WithMany(log => log.CompletedActivities)
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(activity => activity.Title).HasMaxLength(220).IsRequired().HasColumnName("title");

        builder.HasMany(activity => activity.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "completed_activity_readiness_outcomes",
                right => right.HasOne<ReadinessOutcome>()
                    .WithMany()
                    .HasForeignKey("readiness_outcome_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<CompletedActivity>()
                    .WithMany()
                    .HasForeignKey("completed_activity_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("completed_activity_readiness_outcomes");
                    join.HasKey("completed_activity_id", "readiness_outcome_id");
                });

        builder.HasMany(activity => activity.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "completed_activity_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<CompletedActivity>()
                    .WithMany()
                    .HasForeignKey("completed_activity_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("completed_activity_stored_files");
                    join.HasKey("completed_activity_id", "stored_file_id");
                });

        builder.Navigation(activity => activity.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(activity => activity.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(activity => activity.DomainEvents);
    }
}
