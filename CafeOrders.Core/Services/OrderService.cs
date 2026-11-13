using System.Globalization;
using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;

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

    /// <summary>Додає перевірений рядок до замовлення. Рядків з консолі ядро не бачить.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="line">Рядок, уже розібраний барикадою.</param>
    /// <exception cref="DomainRuleException">Замовлення немає або воно вже не нове.</exception>
    public void AddLine(int orderId, OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        Order order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено");
        if (order.Status != OrderStatus.New)
        {
            throw new DomainRuleException(
                "order.editable",
                $"Редагувати можна лише нове замовлення; стан {order.Status}");
        }

        order.AddLine(line);
    }

    /// <summary>Закриває рахунок замовлення: Served → Paid.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="amount">Внесена сума, грн.</param>
    /// <exception cref="DomainRuleException">
    /// Замовлення немає (order.exists), стан не дозволяє оплату (order.payable)
    /// або внесено менше за рахунок (order.amount).
    /// </exception>
    public void Pay(int orderId, decimal amount)
    {
        Order order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено");
        if (order.Status != OrderStatus.Served)
        {
            throw new DomainRuleException("order.payable", $"Стан {order.Status} не дозволяє оплату");
        }

        decimal total = TotalOf(order);
        if (amount < total)
        {
            throw new DomainRuleException(
                "order.amount",
                string.Format(CultureInfo.InvariantCulture, "Сплачено {0:F2}, треба {1:F2}", amount, total));
        }

        order.Pay();
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
