namespace CafeOrders.Legacy;

public interface IDiscountRule
{
    // null — правило до гостя не застосовується
    decimal? RateFor(Visitor c);
}

public sealed class VipRule : IDiscountRule
{
    public decimal? RateFor(Visitor c) =>
        c.Kind == ClientKind.Vip ? PricingRules.VipRate : null;
}

public sealed class StaffRule : IDiscountRule
{
    public decimal? RateFor(Visitor c) =>
        c.Kind == ClientKind.Staff ? PricingRules.StaffRate : null;
}

public sealed class LoyalRule : IDiscountRule
{
    public decimal? RateFor(Visitor c) =>
        c.IsLoyal ? PricingRules.LoyalRate : null;
}
