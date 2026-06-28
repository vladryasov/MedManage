using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("PurchaseRequests");

        builder.HasKey(pr => pr.PurchaseRequestId);

        builder.Property(pr => pr.Message)
            .HasMaxLength(1000);

        builder.Property(pr => pr.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pr => pr.CreatedAt)
            .IsRequired();

        builder.Property(pr => pr.UpdatedAt)
            .IsRequired();

        builder.HasOne(pr => pr.Announcement)
            .WithMany(a => a.PurchaseRequests)
            .HasForeignKey(pr => pr.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.BuyerUser)
            .WithMany(u => u.PurchaseRequestsAsBuyer)
            .HasForeignKey(pr => pr.BuyerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.SellerUser)
            .WithMany(u => u.PurchaseRequestsAsSeller)
            .HasForeignKey(pr => pr.SellerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.SellerUserId);
        builder.HasIndex(pr => pr.BuyerUserId);
        builder.HasIndex(pr => pr.AnnouncementId);
    }
}
