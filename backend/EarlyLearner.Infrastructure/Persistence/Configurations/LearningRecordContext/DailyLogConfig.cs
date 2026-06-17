using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class DailyLogConfig : IEntityTypeConfiguration<DailyLog>
{
    public void Configure(EntityTypeBuilder<DailyLog> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(DailyLog)));

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .ValueGeneratedNever();

        builder.Property(log => log.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(log => log.Household)
            .WithMany()
            .HasForeignKey(log => log.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(log => log.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne(log => log.Child)
            .WithMany()
            .HasForeignKey(log => log.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(log => log.LogDate).IsRequired();

        builder.HasMany(log => log.CompletedActivities)
            .WithOne(activity => activity.DailyLog)
            .HasForeignKey(activity => activity.DailyLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.ReadingEntries)
            .WithOne(entry => entry.DailyLog)
            .HasForeignKey(entry => entry.DailyLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.RoutineEntries)
            .WithOne(entry => entry.DailyLog)
            .HasForeignKey(entry => entry.DailyLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(log => log.StoredFiles)
            .WithMany()
            .UsingEntity<DailyLogStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.DailyLog)
                    .WithMany()
                    .HasForeignKey(join => join.DailyLogId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.DailyLogId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(DailyLogStoredFile)));
                    join.Property(item => item.DailyLogId)
                        .HasConversion(id => id.Value, value => new DailyLogId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(log => log.CompletedActivities).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.ReadingEntries).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.RoutineEntries).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(log => log.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(log => new { log.HouseholdId, log.ChildId, log.LogDate }).IsUnique();
        builder.Ignore(log => log.DomainEvents);
    }

    private sealed class DailyLogStoredFile
    {
        public DailyLogId DailyLogId { get; set; }
        public DailyLog DailyLog { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
