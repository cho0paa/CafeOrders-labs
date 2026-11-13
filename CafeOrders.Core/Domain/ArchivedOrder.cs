namespace CafeOrders.Core.Domain;

// Архівне замовлення НЕ є замовленням: воно лише показує його дані (композиція замість успадкування).

/// <summary>Архівне замовлення лише для читання.</summary>
public sealed class ArchivedOrder
{
    private readonly Order _order;

    /// <summary>Ініціалізує архівний запис над закритим замовленням.</summary>
    /// <param name="order">Замовлення, що архівується.</param>
    /// <param name="archivedAt">Дата архівування.</param>
    public ArchivedOrder(Order order, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(order);
        _order = order;
        ArchivedAt = archivedAt;
    }

    /// <summary>Отримує дату архівування.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Отримує номер замовлення.</summary>
    public int Id => _order.Id;

    /// <summary>Отримує стан замовлення на момент архівування.</summary>
    public OrderStatus Status => _order.Status;

    /// <summary>Отримує рядки замовлення лише для читання.</summary>
    public IReadOnlyList<OrderLine> Lines => _order.Lines;

    // Методу AddLine немає взагалі: порушити контракт нічим.
}
