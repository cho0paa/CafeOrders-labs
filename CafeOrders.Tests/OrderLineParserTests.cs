using CafeOrders.App.Input;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;

namespace CafeOrders.Tests;

// Розбирач — межа модуля: сюди потрапляє сирий ввід касира, тому тут перевірено некоректні рядки.
public class OrderLineParserTests
{
    [Theory]
    [InlineData("", "2", "55.50")]
    [InlineData("   ", "2", "55.50")]
    [InlineData("CAP-001", "", "55.50")]
    [InlineData("CAP-001", "0", "55.50")]
    [InlineData("CAP-001", "-2", "55.50")]
    [InlineData("CAP-001", "2", "-55.50")]
    [InlineData("CAP-001", "дві", "55.50")]
    [InlineData("CAP-001", "2", "55,50")]
    public void Parse_InvalidInput_ReturnsFailure(string menuCode, string quantity, string price)
    {
        Result<OrderLine> result = OrderLineParser.Parse(menuCode, quantity, price);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void Parse_ValidInput_ReturnsLineWithInvariantPrice()
    {
        Result<OrderLine> result = OrderLineParser.Parse(" CAP-001 ", "2", "55.50");

        Assert.True(result.IsSuccess);
        Assert.Equal(new OrderLine("CAP-001", 2, 55.50m), result.Value);
    }
}
