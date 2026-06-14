using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using MedManage.Persistence.Migrations;
using MedManage.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MedManageDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MedManageDbContext).Assembly.FullName)));

        return services;
    }

    public static IServiceCollection AddInfrastructureRepositoriesServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        services.AddScoped<MigrationService>();

        return services;
    }
}
