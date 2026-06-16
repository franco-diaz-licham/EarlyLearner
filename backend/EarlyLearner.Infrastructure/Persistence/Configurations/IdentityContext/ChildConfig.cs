using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class ChildConfig : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> builder)
    {
        builder.ToTable("children");

        builder.HasKey(child => child.Id);

        builder.Property(child => child.Id)
            .HasConversion(id => id.Value, value => new ChildId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<HouseholdId>("HouseholdId")
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany(household => household.Children)
            .HasForeignKey("HouseholdId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(child => child.GivenName)
            .HasMaxLength(120)
            .IsRequired()
            .HasColumnName("given_name");

        builder.Property(child => child.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        builder.Property(child => child.IsArchived)
            .HasColumnName("is_archived")
            .IsRequired();

        builder.HasIndex("HouseholdId", nameof(Child.GivenName));
        builder.Ignore(child => child.DomainEvents);
    }
}
