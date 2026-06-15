using MedManage.Domain.Entities;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для инвентаризации.
/// </summary>
public class InventoryDTO
{
    /// <summary>
    /// Количество товара на складе.
    /// </summary>
    public int QuantityInStock { get; set; }

    /// <summary>
    /// Дата и время последнего обновления инвентаризации.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}