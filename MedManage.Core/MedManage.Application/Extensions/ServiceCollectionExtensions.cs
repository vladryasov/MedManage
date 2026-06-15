using MedManage.Application.Interfaces;
using MedManage.Application.Mappings;
using MedManage.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Application.Extensions;

/// <summary>
/// Класс расширений для регистрации сервисов приложения.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Метод для регистрации всех основных сервисов и профилей AutoMapper.
    /// </summary>
    /// <param name="services">Коллекция сервисов для добавления зависимостей.</param>
    public static void AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IUserService, UserService>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<InventoryMappingProfile>();
            cfg.AddProfile<OrganizationMappingProfile>();
            cfg.AddProfile<AnnouncementMappingProfile>();
        });
        /*services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
        services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddAutoMapper(typeof(InventoryMappingProfile));
        services.AddAutoMapper(typeof(OrganizationMappingProfile));
        services.AddAutoMapper(typeof(AnnouncementMappingProfile));*/
    }
}