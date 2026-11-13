using System.Globalization;
using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;
using Microsoft.Extensions.Logging;

namespace CafeOrders.Core.Services;

/// <summary>Прикладний сервіс замовлень: сценарії оформлення, редагування, оплати й скасування.</summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<OrderService> _log;

    /// <summary>Ініціалізує сервіс залежностями; конкретні класи обирає композиційний корінь.</summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціни рядка.</param>
    /// <param name="notifier">Сповіщення гостя.</param>
    /// <param name="log">Журнал сервісу.</param>
    public OrderService(
        IOrderRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<OrderService> log)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>Оформлює замовлення: зберігає його й сповіщає гостя про суму.</summary>
    /// <param name="order">Замовлення.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", order.Id);
        decimal total = TotalOf(order);
        _repository.Add(order);
        _notifier.Notify(
            order.GuestId,
            string.Format(CultureInfo.InvariantCulture, "Замовлення № {0}: {1:F2} грн", order.Id, total));
        _log.LogInformation("Замовлення оформлено, рядків {LineCount}", order.Lines.Count);
        return total;
    }

    /// <summary>Шукає замовлення; «не знайдено» — нормальний результат запиту, а не помилка.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <returns>Замовлення або null.</returns>
    public Order? Find(int orderId) => _repository.GetById(orderId);

    /// <summary>Додає перевірений рядок до замовлення. Рядків з консолі ядро не бачить.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="line">Рядок, уже розібраний барикадою.</param>
    /// <exception cref="DomainRuleException">Замовлення немає або воно вже не нове.</exception>
    public void AddLine(int orderId, OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        Order order = Require(orderId);
        if (order.Status != OrderStatus.New)
        {
            _log.LogWarning("Спроба змінити замовлення у стані {Status}", order.Status);
            throw new DomainRuleException(
                "order.editable",
                $"Редагувати можна лише нове замовлення; стан {order.Status}");
        }

        order.AddLine(line);
        _log.LogDebug("Додано {MenuCode} x {Quantity}", line.MenuCode, line.Quantity);
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
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        _log.LogDebug("Початок оплати, сума {Amount}", amount);
        Order order = Require(orderId);
        if (order.Status != OrderStatus.Served)
        {
            _log.LogWarning("Оплату відхилено: стан {Status}", order.Status);
            throw new DomainRuleException("order.payable", $"Стан {order.Status} не дозволяє оплату");
        }

        decimal total = TotalOf(order);
        if (amount < total)
        {
            _log.LogWarning("Оплату відхилено: сплачено {Amount}, треба {Total}", amount, total);
            throw new DomainRuleException(
                "order.amount",
                string.Format(CultureInfo.InvariantCulture, "Сплачено {0:F2}, треба {1:F2}", amount, total));
        }

        if (amount > total)
        {
            _log.LogWarning("Переплата {Extra}", amount - total);
        }

        order.Pay();
        _log.LogInformation("Оплачено на {Amount}", amount);
    }

    /// <summary>Скасовує замовлення.</summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="reason">Причина скасування для журналу.</param>
    /// <exception cref="DomainRuleException">Замовлення не знайдено.</exception>
    /// <exception cref="IOException">Збій сховища; виняток іде вгору зі збереженим стеком.</exception>
    public void Cancel(int orderId, string reason)
    {
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        try
        {
            Require(orderId).Cancel();
            _log.LogInformation("Скасовано, причина: {Reason}", reason);
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Збій сховища під час скасування {OrderId}", orderId);
            throw;
        }
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

    private Order Require(int orderId)
    {
        Order? order = _repository.GetById(orderId);
        if (order is null)
        {
            _log.LogWarning("Замовлення {OrderId} не знайдено", orderId);
            throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено");
        }

        return order;
    }
}
