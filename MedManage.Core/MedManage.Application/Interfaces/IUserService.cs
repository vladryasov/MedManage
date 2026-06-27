using MedManage.Application.DTOs;
using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO> CreateUserAsync(CreateUserRequest request);
    Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync();
    Task UpdateUserInfoAsync(UserDTO updatedUser);
    Task UpdateUserRoleAsync(Guid userId, UserRole newRole);
    Task UpdateUserPhoneNumberAsync(Guid userId, string phoneNumber);
    Task DeleteUserAsync(Guid userId);
    Task<UserDTO> GetCurrentUserAsync();
    string GetUserNameFromToken();
}
