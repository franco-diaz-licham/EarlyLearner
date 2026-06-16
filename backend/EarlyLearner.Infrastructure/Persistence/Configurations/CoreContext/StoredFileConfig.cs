using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.CoreContext.ValueObjects;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyLearner.Infrastructure.Persistence.Configurations.CoreContext;

public sealed class StoredFileConfig : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("stored_files");

        builder.HasKey(file => file.Id);

        builder.Property(file => file.Id)
            .HasConversion(id => id.Value, value => new StoredFileId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(file => file.HouseholdId)
            .HasConversion(id => id.Value, value => new HouseholdId(value))
            .HasColumnName("household_id")
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(file => file.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(file => file.StorageKey).HasMaxLength(500).IsRequired().HasColumnName("storage_key");
        builder.Property(file => file.FileName).HasMaxLength(260).IsRequired().HasColumnName("file_name");
        builder.Property(file => file.ContentType).HasMaxLength(160).IsRequired().HasColumnName("content_type");
        builder.Property(file => file.SizeInBytes).IsRequired().HasColumnName("size_in_bytes");

        builder.Property(file => file.MediaType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("media_type");

        builder.Property(file => file.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(file => file.UploadedAt).IsRequired().HasColumnName("uploaded_at");
        builder.HasIndex(file => new { file.HouseholdId, file.StorageKey }).IsUnique();
        builder.Ignore(file => file.DomainEvents);
    }
}
