using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Tests;

// Стаб: жодного стану, лише наперед задана відповідь.
internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(OrderLine line) => _price;
}
