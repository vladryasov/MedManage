using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MedManage.Persistence.Migrations.Factories;

public class MedManageDbContextFactory : IDesignTimeDbContextFactory<MedManageDbContext>
{
    public MedManageDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "MedManage.WebAPI"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MedManageDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(MedManageDbContext).Assembly.FullName));

        return new MedManageDbContext(optionsBuilder.Options);
    }
}
