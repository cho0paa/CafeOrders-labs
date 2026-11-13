using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Pricing;

/// <summary>Звичайна ціна без знижки.</summary>
public sealed class RegularPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return line.Amount;
    }
}
