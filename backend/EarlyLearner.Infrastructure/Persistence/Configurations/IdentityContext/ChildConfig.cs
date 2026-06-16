using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class ChildConfig : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(Child)));

        builder.HasKey(child => child.Id);

        builder.Property(child => child.Id)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .ValueGeneratedNever();

        builder.Property(child => child.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(child => child.Household)
            .WithMany(household => household.Children)
            .HasForeignKey(child => child.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(child => child.GivenName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(child => child.DateOfBirth)
            .IsRequired();

        builder.Property(child => child.IsArchived)
            .IsRequired();

        builder.HasIndex(child => new { child.HouseholdId, child.GivenName });
        builder.Ignore(child => child.DomainEvents);
    }
}
