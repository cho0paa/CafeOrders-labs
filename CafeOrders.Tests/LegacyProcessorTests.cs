using CafeOrders.Core.Legacy;

namespace CafeOrders.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<BillItem> lines = new()
        {
            new BillItem { MenuCode = "CAP-001", Quantity = 12, UnitPrice = 100m },
            new BillItem { MenuCode = "CAK-002", Quantity = 1, UnitPrice = 250m },
        };

        decimal total = LegacyProcessor.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<BillItem> lines = new()
        {
            new BillItem { MenuCode = "CAP-001", Quantity = 2, UnitPrice = 200m },
        };

        decimal total = LegacyProcessor.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            "Paid",
            false,
            60m,
            out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Served", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3,
            "Коваль",
            "k@ex.com",
            false,
            new List<BillItem>(),
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string _);

        Assert.Equal(-1m, total);
    }
}
