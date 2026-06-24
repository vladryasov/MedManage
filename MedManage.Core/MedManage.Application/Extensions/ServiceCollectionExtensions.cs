using MedManage.Application.Interfaces;
using MedManage.Application.Mappings;
using MedManage.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpClient<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<InventoryMappingProfile>();
            cfg.AddProfile<OrganizationMappingProfile>();
            cfg.AddProfile<AnnouncementMappingProfile>();
        });
    }
}