using System.Data;
using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedManage.Persistence.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Product> Products { get; }
    DbSet<Inventory> Inventories { get; }
    DbSet<Announcement> Announcements { get; }
    DbSet<NotificationOutbox> NotificationOutbox { get; }
    DbSet<PurchaseRequest> PurchaseRequests { get; }
    DbSet<InAppNotification> InAppNotifications { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();

    IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);
    Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.RepeatableRead,
        CancellationToken cancellationToken = default);
}
