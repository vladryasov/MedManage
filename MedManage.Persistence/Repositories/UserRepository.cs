using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью пользователей.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly MedManageDbContext _context;

    public UserRepository(MedManageDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
