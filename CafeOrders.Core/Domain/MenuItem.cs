namespace CafeOrders.Core.Domain;

/// <summary>Позиція меню кав’ярні: напій або страва.</summary>
public sealed class MenuItem
{
    public string Code { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int PortionGrams { get; init; }
}
