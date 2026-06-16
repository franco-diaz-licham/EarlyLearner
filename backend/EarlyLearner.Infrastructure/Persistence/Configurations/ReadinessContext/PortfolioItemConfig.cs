using EarlyLearner.Domain.CoreContext.Entities;
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

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(item => item.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(item => item.ChildId)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .IsRequired();

        builder.HasOne<Child>()
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
            .UsingEntity<Dictionary<string, object>>(
                "portfolio_item_readiness_outcomes",
                right => right.HasOne<ReadinessOutcome>()
                    .WithMany()
                    .HasForeignKey("readiness_outcome_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<PortfolioItem>()
                    .WithMany()
                    .HasForeignKey("portfolio_item_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.ToTable("portfolio_item_readiness_outcomes");
                    join.HasKey("portfolio_item_id", "readiness_outcome_id");
                });

        builder.HasMany(item => item.StoredFiles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "portfolio_item_stored_files",
                right => right.HasOne<StoredFile>()
                    .WithMany()
                    .HasForeignKey("stored_file_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<PortfolioItem>()
                    .WithMany()
                    .HasForeignKey("portfolio_item_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => {
                    join.ToTable("portfolio_item_stored_files");
                    join.HasKey("portfolio_item_id", "stored_file_id");
                });

        builder.Navigation(item => item.ReadinessOutcomes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(item => item.StoredFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(item => new { item.HouseholdId, item.ChildId, item.CapturedOn });
        builder.Ignore(item => item.DomainEvents);
    }
}
