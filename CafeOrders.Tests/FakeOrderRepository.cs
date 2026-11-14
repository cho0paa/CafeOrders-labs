using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Tests;

// Фейк: спрощене, але робоче сховище на словнику в пам’яті.
internal sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _items = new();

    public int AddCalls { get; private set; }

    public void Add(Order order)
    {
        _items[order.Id] = order;
        AddCalls++;
    }

    public Order? GetById(int id) =>
        _items.TryGetValue(id, out Order? found) ? found : null;

    public IReadOnlyList<Order> GetAll() => _items.Values.ToList();
}
