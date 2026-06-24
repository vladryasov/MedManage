using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MedManage.Persistence.Seeders;

public class AdminUserSeeder : IDataSeeder
{
    public const string DefaultUserName = "admin";
    public const string DefaultFullName = "System Administrator";
    public const string DefaultPhoneNumber = "+79000000000";

    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminUserSeeder(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var adminEmail = _configuration["AdminSettings:Email"];

        if (string.IsNullOrWhiteSpace(adminEmail))
            return;

        var adminExists = await _context.Users
            .AnyAsync(u => u.Email == adminEmail, cancellationToken);

        if (adminExists)
            return;

        var admin = new User(
            DefaultUserName,
            DefaultFullName,
            adminEmail,
            UserRole.Admin,
            DefaultPhoneNumber);

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");

        await _context.Users.AddAsync(admin, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
