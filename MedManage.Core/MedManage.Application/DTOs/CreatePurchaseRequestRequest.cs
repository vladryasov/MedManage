namespace MedManage.Application.DTOs;

/// <summary>
/// Запрос на создание запроса на покупку.
/// </summary>
public class CreatePurchaseRequestRequest
{
    public Guid AnnouncementId { get; set; }
    public string? Message { get; set; }
}
