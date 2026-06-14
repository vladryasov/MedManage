using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");

        builder.HasKey(a => a.AnnouncementId);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Content)
            .HasMaxLength(2000);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.Property(a => a.StatusInventory)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.TypeProduct)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Views)
            .IsRequired();

        builder.HasIndex(a => a.CreatedByUserId);

        builder.HasIndex(a => a.OrganizationId);

        builder.HasIndex(a => a.CreatedAt);

        builder.HasIndex(a => new { a.TypeProduct, a.StatusInventory });

        builder.HasOne(a => a.User)
            .WithMany(u => u.Announcements)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Organization)
            .WithMany(o => o.Announcements)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
