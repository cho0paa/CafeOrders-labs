namespace CafeOrders.Core.Domain;

/// <summary>Позиція меню кав’ярні: напій або страва.</summary>
public sealed class MenuItem
{
    /// <summary>Отримує код позиції меню, наприклад ESP-001.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Отримує назву страви або напою.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Отримує ціну однієї порції, грн.</summary>
    public decimal Price { get; init; }

    /// <summary>Отримує вагу (об’єм) порції в грамах.</summary>
    public int PortionGrams { get; init; }
}
