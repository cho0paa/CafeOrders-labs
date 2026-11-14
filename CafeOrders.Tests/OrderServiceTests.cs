using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;
using CafeOrders.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace CafeOrders.Tests;

public class OrderServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        var repository = new FakeOrderRepository();
        Order order = NewOrder(7);
        order.AddLine(new OrderLine("CAP-001", 2, 10m));
        order.AddLine(new OrderLine("CAK-002", 1, 30m));
        repository.Add(order);
        OrderService service = NewService(repository, new StubPricingPolicy(25m));

        decimal actual = service.TotalOf(order);

        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_OrderWithLines_StoresAndNotifiesOnce()
    {
        var repository = new FakeOrderRepository();
        var notifier = new SpyNotifier();
        OrderService service = NewService(repository, new StubPricingPolicy(1m), notifier);
        Order order = NewOrder(1);
        order.AddLine(new OrderLine("CAP-001", 1, 5m));

        service.Place(order);

        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
        Assert.Single(notifier.Messages);
    }

    [Fact]
    public void Pay_UnknownId_ThrowsDomainRule()
    {
        OrderService service = NewService(new FakeOrderRepository(), new StubPricingPolicy(1m));

        var ex = Assert.Throws<DomainRuleException>(() => service.Pay(42, 100m));

        Assert.Equal("order.exists", ex.Rule);
    }

    private static Order NewOrder(int id) =>
        new(id, 5, new DateTimeOffset(2026, 3, 10, 12, 0, 0, TimeSpan.Zero));

    private static OrderService NewService(
        FakeOrderRepository repository, StubPricingPolicy pricing, SpyNotifier? notifier = null) =>
        new(repository, pricing, notifier ?? new SpyNotifier(), NullLogger<OrderService>.Instance);
}
