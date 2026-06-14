using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedManage.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.OrganizationId);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasIndex(o => o.Name);

        builder.HasIndex(o => o.Email)
            .IsUnique();
    }
}
