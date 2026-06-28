using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class NotificationOutboxConfiguration : IEntityTypeConfiguration<NotificationOutbox>
{
    public void Configure(EntityTypeBuilder<NotificationOutbox> builder)
    {
        builder.ToTable("NotificationOutbox");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.RecipientEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasOne(n => n.RecipientUser)
            .WithMany()
            .HasForeignKey(n => n.RecipientUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.CreatedAt);
    }
}
