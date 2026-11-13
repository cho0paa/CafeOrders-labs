using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Abstractions;

/// <summary>Політика ціни рядка замовлення: одне правило — одна реалізація.</summary>
public interface IPricingPolicy
{
    /// <summary>Обчислює ціну рядка за правилом політики.</summary>
    /// <param name="line">Рядок замовлення.</param>
    /// <returns>Ціна рядка до сплати, грн.</returns>
    decimal PriceOf(OrderLine line);
}
