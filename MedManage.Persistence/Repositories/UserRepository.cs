using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью пользователей.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context)
    {
        _context = context;
    }

    [Cache("UserById:{userId}", ExpirationSeconds = 1800)] // 30 минут
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    [Cache("AllUsers", ExpirationSeconds = 600)] // 10 минут
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    [Cache("AllUsers", ExpirationSeconds = 600)]
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    [Transactional]
    [CacheInvalidate("AllUsers", "UserById:{user.Id}")]
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    [Cache("UserById:{userId}", ExpirationSeconds = 1800)]
    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    [Transactional]
    [CacheInvalidate("AllUsers")] // Сбрасываем список, т.к. появился новый пользователь
    public async Task<User> AddAsync(
        string userName,
        string fullName,
        string email,
        UserRole role,
        string phoneNumber,
        string? passwordHash = null,
        Guid? organizationId = null)
    {
        var user = new User(userName, fullName, email, role, phoneNumber, passwordHash, organizationId);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}