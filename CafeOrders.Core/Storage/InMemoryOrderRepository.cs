using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Storage;

/// <summary>Сховище замовлень у пам’яті на словнику за номером замовлення.</summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = new();

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Замовлення з таким номером уже є.</exception>
    public void Add(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Замовлення № {order.Id} уже існує.");
        }
    }

    /// <inheritdoc/>
    public Order? GetById(int id) => _orders.TryGetValue(id, out Order? order) ? order : null;

    /// <inheritdoc/>
    public IReadOnlyList<Order> GetAll() => _orders.Values.ToList();
}
