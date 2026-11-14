namespace CafeOrders.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, ClientKind kind, int done)
    {
        decimal rate = RateFor(kind, done);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(ClientKind kind, int done) => kind switch
    {
        ClientKind.Vip => PricingRules.VipRate,
        ClientKind.Staff => PricingRules.StaffRate,
        _ => done > PricingRules.LoyalOrders ? PricingRules.LoyalRate : 0m,
    };
}
