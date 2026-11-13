namespace CafeOrders.Core.Domain;

/// <summary>Гість кав’ярні, на якого оформлюють замовлення.</summary>
public sealed class Guest
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsRegular { get; init; }
}
