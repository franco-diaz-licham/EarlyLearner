using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class HouseholdConfig : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(Household)));

        builder.HasKey(household => household.Id);

        builder.Property(household => household.Id)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .ValueGeneratedNever();

        builder.Property(household => household.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasMany(household => household.Carers)
            .WithOne(carer => carer.Household)
            .HasForeignKey(carer => carer.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(household => household.Children)
            .WithOne(child => child.Household)
            .HasForeignKey(child => child.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(household => household.Invitations)
            .WithOne(invitation => invitation.Household)
            .HasForeignKey(invitation => invitation.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(household => household.Carers).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(household => household.Children).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(household => household.Invitations).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(household => household.CreatedOn).IsRequired();
        builder.Property(household => household.UpdatedOn).IsRequired(false);
        builder.Ignore(household => household.DomainEvents);
    }
}
