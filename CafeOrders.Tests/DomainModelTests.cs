using CafeOrders.Core.Domain;

namespace CafeOrders.Tests;

public class DomainModelTests
{
    [Fact]
    public void Order_FullLifecycle_EndsInPaid()
    {
        Order order = NewOrder();
        order.AddLine(new OrderLine("CAP-001", 1, 55m));

        order.StartPreparing();
        order.Serve();
        order.Pay();

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void StartPreparing_EmptyOrder_Throws()
    {
        Order order = NewOrder();

        Assert.Throws<InvalidOperationException>(order.StartPreparing);
    }

    [Fact]
    public void Serve_NewOrder_Throws()
    {
        Order order = NewOrder();

        Assert.Throws<InvalidOperationException>(order.Serve);
    }

    [Fact]
    public void Pay_NotServed_Throws()
    {
        Order order = NewOrder();

        Assert.Throws<InvalidOperationException>(order.Pay);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Cancel_NewOrPreparing_BecomesCancelled(bool startPreparing)
    {
        Order order = NewOrder();
        order.AddLine(new OrderLine("CAP-001", 1, 55m));
        if (startPreparing)
        {
            order.StartPreparing();
        }

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ServedOrder_Throws()
    {
        Order order = NewOrder();
        order.AddLine(new OrderLine("CAP-001", 1, 55m));
        order.StartPreparing();
        order.Serve();

        Assert.Throws<InvalidOperationException>(order.Cancel);
    }

    [Fact]
    public void Order_ZeroId_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(0, 1, DateTimeOffset.UnixEpoch));
    }

    [Fact]
    public void Guest_EmailWithoutAt_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Guest(1, "Іван", "ivan.mail"));
    }

    [Fact]
    public void Guest_Valid_KeepsData()
    {
        var guest = new Guest(1, "Іван", "i@ex.com") { IsRegular = true };

        Assert.Equal("Іван", guest.Name);
        Assert.Equal("i@ex.com", guest.Email);
        Assert.True(guest.IsRegular);
        Assert.Equal(1, guest.Id);
    }

    [Fact]
    public void MenuItem_ChangePriceAndDisable_UpdatesState()
    {
        var item = new MenuItem("CAP-001", "Капучино", 55m, 250);

        item.ChangePrice(60m);
        item.Disable();

        Assert.Equal(60m, item.Price);
        Assert.False(item.IsAvailable);
        Assert.Equal("Капучино", item.Title);
        Assert.Equal(250, item.PortionGrams);
        item.Enable();
        Assert.True(item.IsAvailable);
        Assert.Equal("CAP-001", item.Code);
    }

    [Fact]
    public void MenuItem_ZeroPrice_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MenuItem("CAP-001", "Капучино", 0m, 250));
    }

    [Fact]
    public void ArchivedOrder_ShowsOrderData()
    {
        Order order = NewOrder();
        order.AddLine(new OrderLine("CAP-001", 2, 55m));
        var archivedAt = new DateOnly(2026, 3, 11);

        var archived = new ArchivedOrder(order, archivedAt);

        Assert.Equal(order.Id, archived.Id);
        Assert.Equal(OrderStatus.New, archived.Status);
        Assert.Single(archived.Lines);
        Assert.Equal(archivedAt, archived.ArchivedAt);
    }

    private static Order NewOrder() => new(1, 5, new DateTimeOffset(2026, 3, 10, 12, 0, 0, TimeSpan.Zero));
}
