using Castle.DynamicProxy;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Repositories;
using MedManage.Persistence.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Persistence.Extensions;

internal static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddProxiedRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ProxyGenerator>();
        services.AddScoped<TransactionInterceptor>();
        services.AddScoped<CachingInterceptor>();

        services.AddScoped<IUserRepository>(provider =>
            CreateProxiedRepository<IUserRepository, UserRepository>(provider,
            provider.GetRequiredService<CachingInterceptor>(),
            provider.GetRequiredService<TransactionInterceptor>()));

        services.AddScoped<IAnnouncementRepository>(provider =>
            CreateProxiedRepository<IAnnouncementRepository, AnnouncementRepository>(provider,
            provider.GetRequiredService<CachingInterceptor>(),
            provider.GetRequiredService<TransactionInterceptor>()));

        services.AddScoped<IOrganizationRepository>(provider =>
            CreateProxiedRepository<IOrganizationRepository, OrganizationRepository>(provider,
            provider.GetRequiredService<CachingInterceptor>(),
            provider.GetRequiredService<TransactionInterceptor>()));

        services.AddScoped<IPurchaseRequestRepository>(provider =>
            CreateProxiedRepository<IPurchaseRequestRepository, PurchaseRequestRepository>(provider,
            provider.GetRequiredService<TransactionInterceptor>()));

        services.AddScoped<IInAppNotificationRepository>(provider =>
            CreateProxiedRepository<IInAppNotificationRepository, InAppNotificationRepository>(provider,
            provider.GetRequiredService<CachingInterceptor>(),
            provider.GetRequiredService<TransactionInterceptor>()));
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        return services;
    }

    private static TInterface CreateProxiedRepository<TInterface, TImplementation>(
        IServiceProvider provider,
        params IInterceptor[] interceptors)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
        var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
        return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(target, interceptors);
    }
}
