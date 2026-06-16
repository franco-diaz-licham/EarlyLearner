using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class HouseholdConfig : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.ToTable("households");

        builder.HasKey(household => household.Id);

        builder.Property(household => household.Id)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(household => household.Name)
            .HasMaxLength(160)
            .IsRequired()
            .HasColumnName("name");

        builder.HasMany(household => household.Carers)
            .WithOne()
            .HasForeignKey("HouseholdId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(household => household.Children)
            .WithOne()
            .HasForeignKey("HouseholdId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(household => household.Carers).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(household => household.Children).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(household => household.DomainEvents);
    }
}
