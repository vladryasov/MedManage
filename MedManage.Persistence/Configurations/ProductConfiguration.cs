using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.ExpirationDate)
            .IsRequired();

        builder.HasIndex(p => p.OrganizationId);

        builder.HasIndex(p => p.Type);

        builder.HasIndex(p => new { p.OrganizationId, p.Name });

        builder.HasOne(p => p.Organization)
            .WithMany(o => o.Products)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Inventory)
            .WithOne(i => i.Product)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
