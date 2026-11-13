using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;

namespace CafeOrders.Tests;

public class DocumentTotalCalculatorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { MenuCode = "CAP-001", Quantity = 12, UnitPrice = 100m },
            new OrderLine { MenuCode = "CAK-002", Quantity = 1, UnitPrice = 250m },
        };

        decimal total = DocumentTotalCalculator.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            string.Empty,
            OrderStatus.New,
            false,
            60m,
            out OrderStatus next);

        Assert.Equal(1292.70m, total);
        Assert.Equal(OrderStatus.Paid, next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { MenuCode = "CAP-001", Quantity = 2, UnitPrice = 200m },
        };

        decimal total = DocumentTotalCalculator.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            OrderStatus.Paid,
            false,
            60m,
            out OrderStatus next);

        Assert.Equal(420m, total);
        Assert.Equal(OrderStatus.Served, next);
    }

    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            DocumentGuards.EnsureLinesValid(new List<OrderLine>()));
    }

    [Fact]
    public void Calculate_ZeroQuantity_ThrowsRange()
    {
        OrderLine line = new() { MenuCode = "CAP-001", Quantity = 0, UnitPrice = 10m };

        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentGuards.EnsureLineValid(line));
    }
}
