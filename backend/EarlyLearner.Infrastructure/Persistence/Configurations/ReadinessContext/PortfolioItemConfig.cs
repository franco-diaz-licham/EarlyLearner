using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.ReadinessContext;

public sealed class PortfolioItemConfig : IEntityTypeConfiguration<PortfolioItem>
{
    public void Configure(EntityTypeBuilder<PortfolioItem> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(PortfolioItem)));

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasConversion(id => id.Value, value => new PortfolioItemId(value))
            .ValueGeneratedNever();

        builder.Property(item => item.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(item => item.Household)
            .WithMany()
            .HasForeignKey(item => item.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(item => item.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne(item => item.Child)
            .WithMany()
            .HasForeignKey(item => item.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(item => item.CapturedOn).IsRequired();
        builder.Property(item => item.Caption).HasMaxLength(1200).IsRequired();

        builder.OwnsOne(
            item => item.Source,
            source => {
                source.Property(value => value.SourceType)
                    .HasConversion<string>()
                    .HasMaxLength(60);

                source.Property(value => value.EvidenceRecordId);
                source.Property(value => value.SourceDate);
            });

        builder.HasMany(item => item.ReadinessOutcomes)
            .WithMany()
            .UsingEntity<PortfolioItemReadinessOutcome>(
                right => right
                    .HasOne(join => join.ReadinessOutcome)
                    .WithMany()
                    .HasForeignKey(join => join.ReadinessOutcomeId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.PortfolioItem)
                    .WithMany()
                    .HasForeignKey(join => join.PortfolioItemId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.PortfolioItemId, item.ReadinessOutcomeId });
                    join.HasIndex(item => item.ReadinessOutcomeId);
                    join.ToTable(StringHelpers.Pluralise(nameof(PortfolioItemReadinessOutcome)));
                    join.Property(item => item.PortfolioItemId)
                        .HasConversion(id => id.Value, value => new PortfolioItemId(value));
                    join.Property(item => item.ReadinessOutcomeId)
                        .HasConversion(id => id.Value, value => new ReadinessOutcomeId(value));
                });

        builder.HasMany(item => item.StoredFiles)
            .WithMany()
            .UsingEntity<PortfolioItemStoredFile>(
                right => right
                    .HasOne(join => join.StoredFile)
                    .WithMany()
                    .HasForeignKey(join => join.StoredFileId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(join => join.PortfolioItem)
                    .WithMany()
                    .HasForeignKey(join => join.PortfolioItemId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.HasKey(item => new { item.PortfolioItemId, item.StoredFileId });
                    join.HasIndex(item => item.StoredFileId);
                    join.ToTable(StringHelpers.Pluralise(nameof(PortfolioItemStoredFile)));
                    join.Property(item => item.PortfolioItemId)
                        .HasConversion(id => id.Value, value => new PortfolioItemId(value));
                    join.Property(item => item.StoredFileId)
                        .HasConversion(id => id.Value, value => new StoredFileId(value));
                });

        builder.Navigation(item => item.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(item => item.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(item => new { item.HouseholdId, item.ChildId, item.CapturedOn });
        builder.Ignore(item => item.DomainEvents);
    }

    private sealed class PortfolioItemReadinessOutcome
    {
        public PortfolioItemId PortfolioItemId { get; set; }
        public PortfolioItem PortfolioItem { get; set; } = null!;
        public ReadinessOutcomeId ReadinessOutcomeId { get; set; }
        public ReadinessOutcome ReadinessOutcome { get; set; } = null!;
    }

    private sealed class PortfolioItemStoredFile
    {
        public PortfolioItemId PortfolioItemId { get; set; }
        public PortfolioItem PortfolioItem { get; set; } = null!;
        public StoredFileId StoredFileId { get; set; }
        public StoredFile StoredFile { get; set; } = null!;
    }
}
