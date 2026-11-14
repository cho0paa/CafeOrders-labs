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

    [Fact]
    public void Pay_AmountBelowTotal_ThrowsAmountRule()
    {
        var repository = new FakeOrderRepository();
        Order order = NewOrder(3);
        order.AddLine(new OrderLine("CAP-001", 2, 50m));
        repository.Add(order);
        order.StartPreparing();
        order.Serve();
        OrderService service = NewService(repository, new StubPricingPolicy(50m));

        var ex = Assert.Throws<DomainRuleException>(() => service.Pay(3, 49.99m));

        Assert.Equal("order.amount", ex.Rule);
        Assert.Equal(OrderStatus.Served, order.Status);
    }

    [Fact]
    public void Pay_ServedWithOverpayment_BecomesPaid()
    {
        var repository = new FakeOrderRepository();
        Order order = ServedOrder(repository, 4);
        OrderService service = NewService(repository, new StubPricingPolicy(50m));

        service.Pay(4, 60m);

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void Pay_NewOrder_ThrowsPayableRule()
    {
        var repository = new FakeOrderRepository();
        repository.Add(NewOrder(5));
        OrderService service = NewService(repository, new StubPricingPolicy(1m));

        var ex = Assert.Throws<DomainRuleException>(() => service.Pay(5, 10m));

        Assert.Equal("order.payable", ex.Rule);
    }

    [Fact]
    public void AddLine_NewOrder_AddsLine()
    {
        var repository = new FakeOrderRepository();
        repository.Add(NewOrder(6));
        OrderService service = NewService(repository, new StubPricingPolicy(1m));

        service.AddLine(6, new OrderLine("CAP-001", 1, 55m));

        Assert.Single(service.Find(6)!.Lines);
    }

    [Fact]
    public void AddLine_ServedOrder_ThrowsEditableRule()
    {
        var repository = new FakeOrderRepository();
        ServedOrder(repository, 7);
        OrderService service = NewService(repository, new StubPricingPolicy(1m));

        var ex = Assert.Throws<DomainRuleException>(
            () => service.AddLine(7, new OrderLine("CAK-002", 1, 70m)));

        Assert.Equal("order.editable", ex.Rule);
    }

    [Fact]
    public void Cancel_NewOrder_BecomesCancelled()
    {
        var repository = new FakeOrderRepository();
        Order order = NewOrder(8);
        repository.Add(order);
        OrderService service = NewService(repository, new StubPricingPolicy(1m));

        service.Cancel(8, "гість пішов");

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Constructor_NullRepository_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderService(
            null!, new StubPricingPolicy(1m), new SpyNotifier(), NullLogger<OrderService>.Instance));
    }

    private static Order ServedOrder(FakeOrderRepository repository, int id)
    {
        Order order = NewOrder(id);
        order.AddLine(new OrderLine("CAP-001", 1, 50m));
        repository.Add(order);
        order.StartPreparing();
        order.Serve();
        return order;
    }

    private static Order NewOrder(int id) =>
        new(id, 5, new DateTimeOffset(2026, 3, 10, 12, 0, 0, TimeSpan.Zero));

    private static OrderService NewService(
        FakeOrderRepository repository, StubPricingPolicy pricing, SpyNotifier? notifier = null) =>
        new(repository, pricing, notifier ?? new SpyNotifier(), NullLogger<OrderService>.Instance);
}
