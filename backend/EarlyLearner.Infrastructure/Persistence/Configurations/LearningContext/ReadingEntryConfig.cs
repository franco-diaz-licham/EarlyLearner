using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningContext;

public sealed class ReadingEntryConfig : IEntityTypeConfiguration<ReadingEntry>
{
    public void Configure(EntityTypeBuilder<ReadingEntry> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(ReadingEntry)));

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasConversion(id => id.Value, value => new ReadingEntryId(value))
            .ValueGeneratedNever();

        builder.Property(entry => entry.DailyLogId)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .IsRequired();

        builder.HasOne(entry => entry.DailyLog)
            .WithMany(log => log.ReadingEntries)
            .HasForeignKey(entry => entry.DailyLogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(entry => entry.Title).HasMaxLength(220).IsRequired();
        builder.Property(entry => entry.Author).HasMaxLength(160).IsRequired();
        builder.Property(entry => entry.ChildResponse).HasMaxLength(1200).IsRequired();

        builder.HasMany(entry => entry.StoredFiles)
            .WithMany()
            .UsingEntity<ReadingEntryStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.ReadingEntry)
                    .WithMany()
                    .HasForeignKey(join => join.ReadingEntryId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.ReadingEntryId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(ReadingEntryStoredFile)));
                    join.Property(item => item.ReadingEntryId)
                        .HasConversion(id => id.Value, value => new ReadingEntryId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(entry => entry.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Property(entry => entry.CreatedOn).IsRequired();
        builder.Property(entry => entry.UpdatedOn).IsRequired(false);
        builder.Ignore(entry => entry.DomainEvents);
    }

    private sealed class ReadingEntryStoredFile
    {
        public ReadingEntryId ReadingEntryId { get; set; }
        public ReadingEntry ReadingEntry { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
