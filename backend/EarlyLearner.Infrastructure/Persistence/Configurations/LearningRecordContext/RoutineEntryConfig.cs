using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class RoutineEntryConfig : IEntityTypeConfiguration<RoutineEntry>
{
    public void Configure(EntityTypeBuilder<RoutineEntry> builder)
    {
        builder.ToTable("routine_entries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasConversion(id => id.Value, value => new RoutineEntryId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<DailyLogId>("DailyLogId")
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .HasColumnName("daily_log_id")
            .IsRequired();

        builder.HasOne<DailyLog>()
            .WithMany(log => log.RoutineEntries)
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(entry => entry.RoutineName).HasMaxLength(180).IsRequired().HasColumnName("routine_name");
        builder.Property(entry => entry.Notes).HasMaxLength(1200).IsRequired().HasColumnName("notes");

        builder.HasMany(entry => entry.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "routine_entry_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<RoutineEntry>()
                    .WithMany()
                    .HasForeignKey("routine_entry_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("routine_entry_stored_files");
                    join.HasKey("routine_entry_id", "stored_file_id");
                });

        builder.Navigation(entry => entry.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(entry => entry.DomainEvents);
    }
}
