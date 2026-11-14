using CafeOrders.Core.Pricing;

namespace CafeOrders.Tests;

public class ServiceChargePolicyTests
{
    [Fact]
    public void ChargeFor_NegativeAmount_Throws()
    {
        var policy = new ServiceChargePolicy();

        Assert.Throws<ArgumentOutOfRangeException>(() => policy.ChargeFor(-0.01m, true));
    }

    [Fact]
    public void ChargeFor_ZeroAmountTakeaway_ReturnsZero()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(0m, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void ChargeFor_SmallBillAtTable_ReturnsFivePercent()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(400m, true);

        Assert.Equal(20m, actual);
    }

    [Fact]
    public void ChargeFor_JustBelowLargeBillTakeaway_ReturnsZero()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(999.99m, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void ChargeFor_LargeBillTakeaway_ReturnsTenPercent()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(1000m, false);

        Assert.Equal(100m, actual);
    }

    [Fact]
    public void ChargeFor_LargeBillAtTable_ReturnsFifteenPercent()
    {
        var policy = new ServiceChargePolicy();

        decimal actual = policy.ChargeFor(1000m, true);

        Assert.Equal(150m, actual);
    }
}
