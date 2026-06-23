using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.IdentityContext;

public sealed class HouseholdInvitationConfig : IEntityTypeConfiguration<HouseholdInvitation>
{
    public void Configure(EntityTypeBuilder<HouseholdInvitation> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(HouseholdInvitation)));

        builder.HasKey(invitation => invitation.Id);

        builder.Property(invitation => invitation.Id)
            .HasConversion(id => id.Value, value => new HouseholdInvitationId(value))
            .ValueGeneratedNever();

        builder.Property(invitation => invitation.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(invitation => invitation.Household)
            .WithMany(household => household.Invitations)
            .HasForeignKey(invitation => invitation.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(invitation => invitation.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(invitation => invitation.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(invitation => invitation.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(invitation => invitation.Role)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(invitation => invitation.InvitedByUserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(invitation => invitation.InvitedAt)
            .IsRequired();

        builder.Property(invitation => invitation.ExpiresAt)
            .IsRequired();

        builder.HasOne(invitation => invitation.InvitedByUser)
            .WithMany()
            .HasForeignKey(invitation => invitation.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(invitation => invitation.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(invitation => invitation.AcceptedByUserId)
            .HasConversion(id => id.HasValue ? id.Value.Value : (Guid?)null, value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        builder.Property(invitation => invitation.AcceptedAt)
            .IsRequired(false);

        builder.HasIndex(invitation => new { invitation.HouseholdId, invitation.Email, invitation.Status });
        builder.Ignore(invitation => invitation.DomainEvents);
    }
}
