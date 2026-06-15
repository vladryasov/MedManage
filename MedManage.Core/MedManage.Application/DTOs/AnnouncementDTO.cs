using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для объявления.
/// </summary>
public class AnnouncementDTO
{
    /// <summary>
    /// Уникальный идентификатор объявления.
    /// </summary>
    public Guid AnnouncementId { get; set; } 

    /// <summary>
    /// Заголовок объявления.
    /// </summary>
    public string Title { get; set; } 

    /// <summary>
    /// Содержимое объявления.
    /// </summary>
    public string Content { get; set; } 

    /// <summary>
    /// Дата и время создания объявления.
    /// </summary>
    public DateTimeOffset  CreatedAt { get; set; } = DateTime.UtcNow; 

    /// <summary>
    /// Дата и время истечения объявления (если задано).
    /// </summary>
    public DateTimeOffset ? ExpirationDate { get; set; }

    public Guid CreatedByUserId { get; set; }
    /// <summary>
    /// Статус инвентаризации, связанный с объявлением.
    /// </summary>
    public InventoryStatus StatusInventory { get; set; }

    /// <summary>
    /// Тип продукта, связанный с объявлением.
    /// </summary>
    public ProductType TypeProduct { get; set; }
    
    public DateTimeOffset ? UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Имя пользователя, создавшего объявление.
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// Количество просмотров объявления.
    /// </summary>
    public int Views { get; set; }
}