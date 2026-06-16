using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class CarerConfig : IEntityTypeConfiguration<Carer>
{
    public void Configure(EntityTypeBuilder<Carer> builder)
    {
        builder.ToTable("carers");

        builder.HasKey(carer => carer.Id);

        builder.Property(carer => carer.Id)
            .HasConversion(id => id.Value, value => new CarerId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property<HouseholdId>("HouseholdId")
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany(household => household.Carers)
            .HasForeignKey("HouseholdId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(carer => carer.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(carer => carer.FirstName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("first_name");

        builder.Property(carer => carer.LastName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("last_name");

        builder.Property(carer => carer.Role)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("role");

        builder.HasIndex("HouseholdId", nameof(Carer.UserId)).IsUnique();
        builder.Ignore(carer => carer.DisplayName);
        builder.Ignore(carer => carer.DomainEvents);
    }
}
