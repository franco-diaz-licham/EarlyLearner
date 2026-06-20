using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class CarerConfig : IEntityTypeConfiguration<Carer>
{
    public void Configure(EntityTypeBuilder<Carer> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(Carer)));

        builder.HasKey(carer => carer.Id);

        builder.Property(carer => carer.Id)
            .HasConversion(id => id.Value, value => new CarerId(value))
            .ValueGeneratedNever();

        builder.Property(carer => carer.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(carer => carer.Household)
            .WithMany(household => household.Carers)
            .HasForeignKey(carer => carer.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(carer => carer.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.HasOne(carer => carer.User)
            .WithMany()
            .HasForeignKey(carer => carer.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(carer => carer.Role)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(carer => new { carer.HouseholdId, carer.UserId }).IsUnique();
        builder.Ignore(carer => carer.DomainEvents);
    }
}
