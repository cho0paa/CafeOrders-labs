using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Pricing;

// Кожне правило — окремий клас; наявні класи при додаванні нового правила не змінюються.

/// <summary>Ціна для VIP-гостя: знижка 10 %.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal VipFactor = 0.90m;

    /// <inheritdoc/>
    public decimal PriceOf(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * VipFactor, 2, MidpointRounding.AwayFromZero);
    }
}
