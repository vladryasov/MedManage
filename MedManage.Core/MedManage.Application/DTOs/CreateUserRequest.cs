namespace MedManage.Application.DTOs;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Guid? OrganizationId { get; set; }
}
