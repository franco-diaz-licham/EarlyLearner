using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class DailyLogConfig : IEntityTypeConfiguration<DailyLog>
{
    public void Configure(EntityTypeBuilder<DailyLog> builder)
    {
        builder.ToTable("daily_logs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(log => log.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(log => log.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(log => log.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .HasColumnName("child_id")
            .IsRequired();

        builder.HasOne<Child>()
            .WithMany()
            .HasForeignKey(log => log.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(log => log.LogDate).HasColumnName("log_date").IsRequired();

        builder.HasMany(log => log.CompletedActivities)
            .WithOne()
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.ReadingEntries)
            .WithOne()
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.RoutineEntries)
            .WithOne()
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "daily_log_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<DailyLog>()
                    .WithMany()
                    .HasForeignKey("daily_log_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("daily_log_stored_files");
                    join.HasKey("daily_log_id", "stored_file_id");
                });

        builder.Navigation(log => log.CompletedActivities).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.ReadingEntries).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.RoutineEntries).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(log => new { log.HouseholdId, log.ChildId, log.LogDate }).IsUnique();
        builder.Ignore(log => log.DomainEvents);
    }
}
