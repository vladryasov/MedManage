namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность складского остатка продукта.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Уникальный идентификатор записи инвентаризации.
    /// </summary>
    public Guid InventoryId { get; set; }

    /// <summary>
    /// Количество товара на складе.
    /// </summary>
    public int QuantityInStock { get; set; }

    /// <summary>
    /// Дата и время последнего обновления инвентаризации.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Идентификатор продукта (внешний ключ, уникальный — связь 1:1).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Продукт, связанный с инвентаризацией.
    /// </summary>
    public Product Product { get; set; } = null!;
}
