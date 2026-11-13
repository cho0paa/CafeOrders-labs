namespace CafeOrders.Core.Domain;

/// <summary>Замовлення гостя кав’ярні.</summary>
public sealed class Order
{
    private readonly List<OrderLine> _lines = new();

    public int Id { get; init; }

    public int GuestId { get; init; }

    public OrderStatus Status { get; set; } = OrderStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<OrderLine> Lines => _lines;

    /// <summary>Додає рядок до замовлення.</summary>
    /// <param name="line">Рядок із позицією меню, кількістю та ціною.</param>
    public void AddLine(OrderLine line) => _lines.Add(line);

    /// <summary>Обчислює суму замовлення.</summary>
    /// <returns>Сума всіх рядків замовлення, грн.</returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (OrderLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
