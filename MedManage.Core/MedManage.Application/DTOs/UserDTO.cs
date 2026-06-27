using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

public class UserDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public Guid? OrganizationId { get; set; }
}

/// <summary>
/// Контактный номер