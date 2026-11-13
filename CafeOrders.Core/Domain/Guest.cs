namespace CafeOrders.Core.Domain;

/// <summary>Гість кав’ярні, на якого оформлюють замовлення.</summary>
public sealed class Guest
{
    /// <summary>Отримує ідентифікатор гостя.</summary>
    public int Id { get; init; }

    /// <summary>Отримує ім’я гостя.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Отримує ознаку постійного гостя, що дає право на знижку.</summary>
    public bool IsRegular { get; init; }
}
