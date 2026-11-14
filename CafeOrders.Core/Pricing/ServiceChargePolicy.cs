using CafeOrders.Core.Abstractions;

namespace CafeOrders.Core.Pricing;

/// <summary>Сервісний збір рахунку кав’ярні.</summary>
public sealed class ServiceChargePolicy : IServiceChargePolicy
{
    /// <summary>Частка збору за обслуговування за столиком.</summary>
    public const decimal TableServiceRate = 0.05m;

    /// <summary>Додаткова частка збору для великого рахунку.</summary>
    public const decimal LargeBillRate = 0.10m;

    /// <summary>Сума, від якої рахунок вважається великим, грн.</summary>
    public const decimal LargeBillFrom = 1000m;

    /// <summary>Максимальна сума сервісного збору, грн.</summary>
    public const decimal MaxCharge = 150m;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">Сума від’ємна.</exception>
    public decimal ChargeFor(decimal amount, bool isTableService)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isTableService)
        {
            rate += TableServiceRate;
        }

        if (amount >= LargeBillFrom)
        {
            rate += LargeBillRate;
        }

        decimal charge = decimal.Round(amount * rate, 2);
        if (charge > MaxCharge)
        {
            charge = MaxCharge;
        }

        return charge;
    }
}
