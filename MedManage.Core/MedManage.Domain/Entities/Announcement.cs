using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность объявления.
/// </summary>
public class Announcement
{
    /// <summary>
    /// Уникальный идентификатор объявления.
    /// </summary>
    public Guid AnnouncementId { get; set; }

    /// <summary>
    /// Заголовок объявления.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Содержимое объявления.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Дата и время создания объявления.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Дата и время истечения объявления (если задано).
    /// </summary>
    public DateTimeOffset? ExpirationDate { get; set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего объявление.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Идентификатор организации, к которой относится объявление.
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Статус инвентаризации (фильтр/категория объявления).
    /// </summary>
    public InventoryStatus StatusInventory { get; set; }

    /// <summary>
    /// Тип продукта, связанный с объявлением.
    /// </summary>
    public ProductType TypeProduct { get; set; }

    /// <summary>
    /// Дата и время последнего обновления объявления.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Количество просмотров объявления.
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// Пользователь, создавший объявление.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Организация, к которой относится объявление.
    /// </summary>
    public Organization? Organization { get; set; }
}
