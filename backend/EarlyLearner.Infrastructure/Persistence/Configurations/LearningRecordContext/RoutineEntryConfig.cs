using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class RoutineEntryConfig : IEntityTypeConfiguration<RoutineEntry>
{
    public void Configure(EntityTypeBuilder<RoutineEntry> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(RoutineEntry)));

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasConversion(id => id.Value, value => new RoutineEntryId(value))
            .ValueGeneratedNever();

        builder.Property(entry => entry.DailyLogId)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .IsRequired();

        builder.HasOne(entry => entry.DailyLog)
            .WithMany(log => log.RoutineEntries)
            .HasForeignKey(entry => entry.DailyLogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(entry => entry.RoutineName).HasMaxLength(180).IsRequired();
        builder.Property(entry => entry.Notes).HasMaxLength(1200).IsRequired();

        builder.HasMany(entry => entry.StoredFiles)
            .WithMany()
            .UsingEntity<RoutineEntryStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.RoutineEntry)
                    .WithMany()
                    .HasForeignKey(join => join.RoutineEntryId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.RoutineEntryId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(RoutineEntryStoredFile)));
                    join.Property(item => item.RoutineEntryId)
                        .HasConversion(id => id.Value, value => new RoutineEntryId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(entry => entry.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(entry => entry.DomainEvents);
    }

    private sealed class RoutineEntryStoredFile
    {
        public RoutineEntryId RoutineEntryId { get; set; }
        public RoutineEntry RoutineEntry { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
