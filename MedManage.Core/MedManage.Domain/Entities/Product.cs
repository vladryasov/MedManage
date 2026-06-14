using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность продукта.
/// </summary>
public class Product
{
    /// <summary>
    /// Уникальный идентификатор продукта.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Название продукта.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Тип продукта.
    /// </summary>
    public ProductType Type { get; set; }

    /// <summary>
    /// Цена продукта.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Дата истечения срока годности продукта.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Идентификатор организации-владельца продукта.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Организация, которой принадлежит продукт.
    /// </summary>
    public Organization Organization { get; set; } = null!;

    /// <summary>
    /// Запись об остатках на складе (1:1).
    /// </summary>
    public Inventory? Inventory { get; set; }
}
