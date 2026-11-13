using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Reporting;

namespace CafeOrders.Tests;

public class DocumentTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(OrderStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_SmallOrder_AddsDelivery()
    {
        TotalRequest request = new(
            DocumentId: 2,
            Client: new ClientInfo("Петренко", "p@ex.com", false),
            Lines: new List<OrderLine>
            {
                new() { MenuCode = "CAP-001", Quantity = 2, UnitPrice = 200m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: OrderStatus.Paid,
            DeliveryPrice: 60m);

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(OrderStatus.Served, result.Status);
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

    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = BuildSampleRequest();
        TotalResult result = DocumentTotalCalculator.Calculate(request);

        string text = DocumentReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        using StringWriter writer = new();
        DocumentReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Calculate_CancelledOrder_ReturnsZero()
    {
        TotalRequest request = BuildSampleRequest() with { Status = OrderStatus.Cancelled };

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(0m, result.Total);
        Assert.Equal(OrderStatus.Cancelled, result.Status);
    }

    [Fact]
    public void Calculate_EmailWithoutAt_ThrowsArgument()
    {
        TotalRequest request = BuildSampleRequest() with
        {
            Client = new ClientInfo("Іваненко", "no-mail", true),
        };

        Assert.Throws<ArgumentException>(() => DocumentTotalCalculator.Calculate(request));
    }

    internal static TotalRequest BuildSampleRequest()
    {
        return new TotalRequest(
            DocumentId: 1,
            Client: new ClientInfo("Іваненко", "i@ex.com", true),
            Lines: new List<OrderLine>
            {
                new() { MenuCode = "CAP-001", Quantity = 12, UnitPrice = 100m },
                new() { MenuCode = "CAK-002", Quantity = 1, UnitPrice = 250m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: OrderStatus.New,
            DeliveryPrice: 60m);
    }
}
