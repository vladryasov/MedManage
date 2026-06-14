using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data;

public class MedManageDbContext : DbContext
{
    public MedManageDbContext(DbContextOptions<MedManageDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedManageDbContext).Assembly);
    }
}
