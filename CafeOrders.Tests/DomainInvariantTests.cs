using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Pricing;
using CafeOrders.Core.Services;
using CafeOrders.Core.Storage;

namespace CafeOrders.Tests;

public class DomainInvariantTests
{
    [Fact]
    public void OrderLine_WithZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OrderLine("CAP-001", 0, 10m));
    }

    [Fact]
    public void Order_AfterStartPreparing_RejectsNewLines()
    {
        Order order = NewOrder(1);
        order.AddLine(new OrderLine("CAP-001", 1, 10m));
        order.StartPreparing();

        Assert.Throws<InvalidOperationException>(
            () => order.AddLine(new OrderLine("CAK-002", 1, 5m)));
    }

    [Fact]
    public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        var service = new OrderService(
            new InMemoryOrderRepository(),
            new DiscountPricingPolicy(0.10m),
            new SilentNotifier());
        Order order = NewOrder(2);
        order.AddLine(new OrderLine("CAP-001", 2, 100m));

        decimal total = service.TotalOf(order);

        Assert.Equal(180m, total);
    }

    private static Order NewOrder(int id) =>
        new(id, 100, new DateTimeOffset(2026, 3, 10, 12, 0, 0, TimeSpan.Zero));

    private sealed class SilentNotifier : INotifier
    {
        public void Notify(int guestId, string message)
        {
        }
    }
}
