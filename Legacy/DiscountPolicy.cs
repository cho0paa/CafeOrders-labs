namespace CafeOrders.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, Visitor c)
    {
        decimal rate = RateFor(c);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(Visitor c) => c.Kind switch
    {
        ClientKind.Vip => PricingRules.VipRate,
        ClientKind.Staff => PricingRules.StaffRate,
        _ => c.IsLoyal ? PricingRules.LoyalRate : 0m,
    };
}
