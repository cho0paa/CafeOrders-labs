namespace CafeOrders.Legacy;

public sealed class DiscountPolicy
{
    // Порядок важливий: VIP і персонал мають пріоритет над лояльністю, як у вихідному коді.
    private readonly IDiscountRule[] _rules =
    {
        new VipRule(),
        new StaffRule(),
        new LoyalRule(),
    };

    public decimal For(decimal subtotal, Visitor c)
    {
        decimal rate = RateFor(c);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private decimal RateFor(Visitor c)
    {
        foreach (IDiscountRule rule in _rules)
        {
            decimal? rate = rule.RateFor(c);
            if (rate.HasValue)
            {
                return rate.Value;
            }
        }

        return 0m;
    }
}
