using CafeOrders.Core.Pricing;

namespace CafeOrders.Tests;

public class ServiceChargePolicyTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    [InlineData(1000.01, true, 150)]
    public void ChargeFor_Boundaries_MatchesTable(double amount, bool isTableService, double expected)
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor((decimal)amount, isTableService);

        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void ChargeFor_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        var policy = new ServiceChargePolicy();

        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => policy.ChargeFor((decimal)amount, true));

        Assert.Equal("amount", ex.ParamName);
    }

    [Fact]
    public void ChargeFor_HugeBill_IsCappedAtMax()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(5000m, true);

        Assert.Equal(150m, actual);
    }
}
