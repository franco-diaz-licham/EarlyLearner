using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(User)));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasConversion(id => id.Value, value => new UserId(value))
            .ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(user => user.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
        builder.Ignore(user => user.DisplayName);
        builder.Ignore(user => user.DomainEvents);
    }
}
