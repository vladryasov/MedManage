namespace MedManage.Domain.Enums;

/// <summary>
/// Перечисление статусов инвентаризации.
/// </summary>
public enum InventoryStatus
{
    /// <summary>
    /// Все статусы.
    /// </summary>
    All = 0,

    /// <summary>
    /// Товар в наличии.
    /// </summary>
    InStock = 1,

    /// <summary>
    /// Товар отсутствует на складе.
    /// </summary>
    OutOfStock = 2,

    /// <summary>
    /// Товар пополняется.
    /// </summary>
    Replenishing = 3,

    /// <summary>
    /// Срок годности товара истек.
    /// </summary>
    Expired = 4
}