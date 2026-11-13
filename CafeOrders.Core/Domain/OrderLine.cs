namespace CafeOrders.Core.Domain;

/// <summary>Рядок замовлення: позиція меню, кількість порцій і ціна.</summary>
public sealed class OrderLine
{
    /// <summary>Отримує код позиції меню.</summary>
    public string MenuCode { get; init; } = string.Empty;

    /// <summary>Отримує кількість порцій.</summary>
    public int Quantity { get; init; }

    /// <summary>Отримує ціну порції на момент замовлення, грн.</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>Отримує суму рядка: кількість × ціна.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
