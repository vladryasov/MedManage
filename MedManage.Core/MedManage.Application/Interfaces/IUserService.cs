using MedManage.Application.DTOs;
using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO> CreateUserAsync(CreateUserRequest request);
    Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync();
    Task UpdateUserInfoAsync(UserDTO updatedUser);
    Task UpdateUserRoleAsync(UserDTO updatedUser, UserRole newRole);
    Task UpdateUserPhoneNumberAsync(UserDTO updatedUser);
    Task<UserDTO> GetCurrentUserAsync();
    string GetUserNameFromToken();
}
