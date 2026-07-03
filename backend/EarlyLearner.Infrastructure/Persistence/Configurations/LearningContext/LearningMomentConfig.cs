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

public sealed class LearningMomentConfig : IEntityTypeConfiguration<LearningMoment>
{
    public void Configure(EntityTypeBuilder<LearningMoment> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(LearningMoment)));

        builder.HasKey(moment => moment.Id);

        builder.Property(moment => moment.Id)
            .HasConversion(id => id.Value, value => new LearningMomentId(value))
            .ValueGeneratedNever();

        builder.Property(moment => moment.DailyLogId)
            .HasConversion(id => id.Value, value => new DailyLogId(value))
            .IsRequired();

        builder.HasOne(moment => moment.DailyLog)
            .WithMany(log => log.LearningMoments)
            .HasForeignKey(moment => moment.DailyLogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(moment => moment.Kind).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(moment => moment.Title).HasMaxLength(220).IsRequired();
        builder.Property(moment => moment.Notes).HasMaxLength(2000).IsRequired();

        builder.HasMany(moment => moment.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<LearningMomentReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.LearningMoment)
                    .WithMany()
                    .HasForeignKey(join => join.LearningMomentId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.LearningMomentId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(LearningMomentReadinessOutcome)));
                    join.Property(item => item.LearningMomentId)
                        .HasConversion(id => id.Value, value => new LearningMomentId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.HasMany(moment => moment.StoredFiles)
            .WithMany()
            .UsingEntity<LearningMomentStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.LearningMoment)
                    .WithMany()
                    .HasForeignKey(join => join.LearningMomentId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.LearningMomentId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(LearningMomentStoredFile)));
                    join.Property(item => item.LearningMomentId)
                        .HasConversion(id => id.Value, value => new LearningMomentId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(moment => moment.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(moment => moment.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Property(moment => moment.CreatedOn).IsRequired();
        builder.Property(moment => moment.UpdatedOn).IsRequired(false);
        builder.Ignore(moment => moment.DomainEvents);
    }

    private sealed class LearningMomentReadinessOutcome
    {
        public LearningMomentId LearningMomentId { get; set; }
        public LearningMoment LearningMoment { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }

    private sealed class LearningMomentStoredFile
    {
        public LearningMomentId LearningMomentId { get; set; }
        public LearningMoment LearningMoment { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
