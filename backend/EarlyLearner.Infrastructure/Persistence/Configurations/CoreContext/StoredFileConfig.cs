using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.CoreContext;

public sealed class StoredFileConfig : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable(StringHelpers.Pluralise(nameof(StoredFile)));

        builder.HasKey(file => file.Id);

        builder.Property(file => file.Id)
            .HasConversion(id => id.Value, value => new StoredFileId(value))
            .ValueGeneratedNever();

        builder.Property(file => file.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .IsRequired();

        builder.HasOne(x => x.Household)
            .WithMany()
            .HasForeignKey(file => file.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(file => file.StorageKey).HasMaxLength(500).IsRequired();
        builder.Property(file => file.FileName).HasMaxLength(260).IsRequired();
        builder.Property(file => file.ContentType).HasMaxLength(160).IsRequired();
        builder.Property(file => file.SizeInBytes).IsRequired();

        builder.Property(file => file.MediaType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(file => file.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(file => file.UploadedAt).IsRequired();
        builder.HasIndex(file => new { file.HouseholdId, file.StorageKey }).IsUnique();
        builder.Property(file => file.CreatedOn).IsRequired();
        builder.Property(file => file.UpdatedOn).IsRequired(false);
        builder.Ignore(file => file.DomainEvents);
    }
}
