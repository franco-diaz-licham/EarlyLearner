using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningRecordContext;

public sealed class ReadingEntryConfig : IEntityTypeConfiguration<ReadingEntry>
{
    public void Configure(EntityTypeBuilder<ReadingEntry> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadingEntry)));

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasConversion(id => id.Value, value => new ReadingEntryId(value))
            .ValueGeneratedNever();

        builder.Property<DailyLogId>("DailyLogId")
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .IsRequired();

        builder.HasOne<DailyLog>()
            .WithMany(log => log.ReadingEntries)
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(entry => entry.Title).HasMaxLength(220).IsRequired();
        builder.Property(entry => entry.Author).HasMaxLength(160).IsRequired();
        builder.Property(entry => entry.ChildResponse).HasMaxLength(1200).IsRequired();

        builder.HasMany(entry => entry.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "reading_entry_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<ReadingEntry>()
                    .WithMany()
                    .HasForeignKey("reading_entry_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.ToTable("reading_entry_stored_files");
                    join.HasKey("reading_entry_id", "stored_file_id");
                });

        builder.Navigation(entry => entry.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(entry => entry.DomainEvents);
    }
}
