namespace CafeOrders.Core.Domain;

/// <summary>
/// Незмінний рядок замовлення: позиція меню, кількість порцій і ціна на момент замовлення.
/// </summary>
public sealed record OrderLine
{
    /// <summary>Ініціалізує рядок і перевіряє його інваріант.</summary>
    /// <param name="menuCode">Код позиції меню, непорожній.</param>
    /// <param name="quantity">Кількість порцій, більша за нуль.</param>
    /// <param name="unitPrice">Ціна порції, невід’ємна, грн.</param>
    /// <exception cref="ArgumentException">Код позиції порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість не додатна або ціна від’ємна.</exception>
    public OrderLine(string menuCode, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(menuCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);

        MenuCode = menuCode;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>Отримує код позиції меню.</summary>
    public string MenuCode { get; }

    /// <summary>Отримує кількість порцій.</summary>
    public int Quantity { get; }

    /// <summary>Отримує ціну порції на момент замовлення, грн.</summary>
    public decimal UnitPrice { get; }

    // Обчислюване значення, а не поле: розійтися з кількістю й ціною неможливо.

    /// <summary>Отримує суму рядка: кількість × ціна.</summary>
    public decimal Amount => UnitPrice * Quantity;
}
