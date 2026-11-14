using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;
using CafeOrders.Core.Pricing;
using CafeOrders.Core.Reporting;
using CafeOrders.Core.Storage;

namespace CafeOrders.Tests;

public class InfrastructureTests
{
    [Fact]
    public void InMemoryRepository_DuplicateId_Throws()
    {
        var repository = new InMemoryOrderRepository();
        repository.Add(NewOrder(1));

        Assert.Throws<InvalidOperationException>(() => repository.Add(NewOrder(1)));
    }

    [Fact]
    public void InMemoryRepository_GetAllAndMissing_ReturnsSnapshotAndNull()
    {
        var repository = new InMemoryOrderRepository();
        repository.Add(NewOrder(1));
        repository.Add(NewOrder(2));

        Assert.Equal(2, repository.GetAll().Count);
        Assert.Null(repository.GetById(99));
    }

    [Theory]
    [InlineData(100, 100)]
    [InlineData(55.55, 55.55)]
    public void RegularPolicy_ReturnsAmount(double price, double expected)
    {
        decimal actual = new RegularPricingPolicy().PriceOf(new OrderLine("CAP-001", 1, (decimal)price));

        Assert.Equal((decimal)expected, actual);
    }

    [Fact]
    public void VipPolicy_TakesTenPercentOff()
    {
        decimal actual = new VipPricingPolicy().PriceOf(new OrderLine("CAP-001", 2, 50m));

        Assert.Equal(90m, actual);
    }

    [Fact]
    public void DiscountPolicy_RateAboveHalf_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiscountPricingPolicy(0.51m));
    }

    [Fact]
    public void OrderCsvReport_TwoOrders_WritesHeaderAndRows()
    {
        Order first = NewOrder(1);
        Order second = NewOrder(2);

        string csv = OrderCsvReport.BuildCsv(new[] { first, second }, _ => 10.5m);

        Assert.Equal("id;total;status\n1;10.50;New\n2;10.50;New\n", csv.Replace("\r\n", "\n", StringComparison.Ordinal));
    }

    [Fact]
    public void Result_OkAndFail_CarryValueOrError()
    {
        Result<int> ok = Result.Ok(5);
        Result<int> fail = Result.Fail<int>("погано");

        Assert.True(ok.IsSuccess);
        Assert.Equal(5, ok.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal("погано", fail.Error);
        Assert.True(ok == Result.Ok(5));
        Assert.True(ok != fail);
        Assert.False(ok.Equals((object)fail));
        Assert.Equal(ok.GetHashCode(), Result.Ok(5).GetHashCode());
    }

    [Fact]
    public void DomainRuleException_DefaultConstructors_KeepUnknownRule()
    {
        var inner = new InvalidOperationException("причина");

        Assert.Equal("unknown", new DomainRuleException().Rule);
        Assert.Equal("unknown", new DomainRuleException("текст").Rule);
        Assert.Same(inner, new DomainRuleException("текст", inner).InnerException);
        Assert.Equal("order.paid", new DomainRuleException("order.paid", "текст", inner).Rule);
    }

    private static Order NewOrder(int id) => new(id, 5, new DateTimeOffset(2026, 3, 10, 12, 0, 0, TimeSpan.Zero));
}
