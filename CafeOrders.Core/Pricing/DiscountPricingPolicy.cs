using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Pricing;

/// <summary>Ціна зі знижкою у вигляді частки від суми рядка.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxRate = 0.5m;

    private readonly decimal _rate;

    /// <summary>Ініціалізує політику ставкою знижки.</summary>
    /// <param name="rate">Частка знижки від 0 до 0,5 включно.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ставка поза межами.</exception>
    public DiscountPricingPolicy(decimal rate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rate, MaxRate);
        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2, MidpointRounding.AwayFromZero);
    }
}
