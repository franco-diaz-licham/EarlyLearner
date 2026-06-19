using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningContext;

public sealed class CompletedActivityConfig : IEntityTypeConfiguration<CompletedActivity>
{
    public void Configure(EntityTypeBuilder<CompletedActivity> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(CompletedActivity)));

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.Id)
            .HasConversion(id => id.Value, value => new CompletedActivityId(value))
            .ValueGeneratedNever();

        builder.Property(activity => activity.DailyLogId)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .IsRequired();

        builder.HasOne(activity => activity.DailyLog)
            .WithMany(log => log.CompletedActivities)
            .HasForeignKey(activity => activity.DailyLogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(activity => activity.Title).HasMaxLength(220).IsRequired();

        builder.HasMany(activity => activity.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<CompletedActivityReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.CompletedActivity)
                    .WithMany()
                    .HasForeignKey(join => join.CompletedActivityId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.CompletedActivityId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(CompletedActivityReadinessOutcome)));
                    join.Property(item => item.CompletedActivityId)
                        .HasConversion(id => id.Value, value => new CompletedActivityId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.HasMany(activity => activity.StoredFiles)
            .WithMany()
            .UsingEntity<CompletedActivityStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.CompletedActivity)
                    .WithMany()
                    .HasForeignKey(join => join.CompletedActivityId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.CompletedActivityId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(CompletedActivityStoredFile)));
                    join.Property(item => item.CompletedActivityId)
                        .HasConversion(id => id.Value, value => new CompletedActivityId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(activity => activity.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(activity => activity.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(activity => activity.DomainEvents);
    }

    private sealed class CompletedActivityReadinessOutcome
    {
        public CompletedActivityId CompletedActivityId { get; set; }
        public CompletedActivity CompletedActivity { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }

    private sealed class CompletedActivityStoredFile
    {
        public CompletedActivityId CompletedActivityId { get; set; }
        public CompletedActivity CompletedActivity { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
