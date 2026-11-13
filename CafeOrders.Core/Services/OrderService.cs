using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Services;

/// <summary>Прикладний сервіс замовлень: сценарій оформлення й розрахунок суми.</summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Ініціалізує сервіс залежностями.</summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціни рядка.</param>
    public OrderService(IOrderRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>Оформлює замовлення: зберігає його в сховищі.</summary>
    /// <param name="order">Замовлення.</param>
    public void Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
    }

    /// <summary>Обчислює суму замовлення за політикою ціни. Не змінює замовлення.</summary>
    /// <param name="order">Замовлення.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal TotalOf(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        decimal total = 0m;
        foreach (OrderLine line in order.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }
}
