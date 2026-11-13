using System.Globalization;
using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Services;

/// <summary>Прикладний сервіс замовлень: сценарій оформлення й розрахунок суми.</summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Ініціалізує сервіс залежностями; конкретні класи обирає композиційний корінь.</summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціни рядка.</param>
    /// <param name="notifier">Сповіщення гостя.</param>
    public OrderService(IOrderRepository repository, IPricingPolicy pricing, INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>Оформлює замовлення: зберігає його й сповіщає гостя про суму.</summary>
    /// <param name="order">Замовлення.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        decimal total = TotalOf(order);
        _repository.Add(order);
        _notifier.Notify(
            order.GuestId,
            string.Format(CultureInfo.InvariantCulture, "Замовлення № {0}: {1:F2} грн", order.Id, total));
        return total;
    }

    /// <summary>Додає рядок до замовлення з рядків, введених касиром.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="menuCode">Код позиції меню.</param>
    /// <param name="quantity">Кількість порцій як текст.</param>
    /// <param name="price">Ціна порції як текст.</param>
    public void AddLine(int orderId, string menuCode, string quantity, string price)
    {
        var order = _repository.GetById(orderId);
        var q = int.Parse(quantity);
        var p = decimal.Parse(price);
        order!.AddLine(new OrderLine(menuCode, q, p));
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
