using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.LearningContext;

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

        builder.HasMany(log => log.LearningMoments)
            .WithOne(moment => moment.DailyLog)
            .HasForeignKey(moment => moment.DailyLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(log => log.LearningMoments).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(log => new { log.HouseholdId, log.ChildId, log.LogDate }).IsUnique();
        builder.Property(log => log.CreatedOn).IsRequired();
        builder.Property(log => log.UpdatedOn).IsRequired(false);
        builder.Ignore(log => log.DomainEvents);
    }

}
