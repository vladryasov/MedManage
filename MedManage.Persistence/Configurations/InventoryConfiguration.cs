using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(i => i.InventoryId);

        builder.Property(i => i.QuantityInStock)
            .IsRequired();

        builder.Property(i => i.LastUpdated)
            .IsRequired();

        builder.HasIndex(i => i.ProductId)
            .IsUnique();
    }
}
