namespace CafeOrders.Core.Domain;

/// <summary>Рядок замовлення: позиція меню, кількість порцій і ціна.</summary>
public sealed class OrderLine
{
    public string MenuCode { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Amount => Quantity * UnitPrice;
}
