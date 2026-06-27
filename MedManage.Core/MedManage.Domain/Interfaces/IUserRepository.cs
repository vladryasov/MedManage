using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с пользователями.
/// </summary>
public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);

    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User> GetByIdAsync(Guid userId);

    Task<User?> FindByEmailAsync(string email);

    Task<User?> FindByUserNameAsync(string userName);

    Task<User> AddAsync(
        string userName,
        string fullName,
        string email,
        UserRole role,
        string phoneNumber,
        string? passwordHash = null,
        Guid? organizationId = null);

    Task UpdateAsync(User user);

    Task DeleteAsync(User user);
}
