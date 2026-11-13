using System.Globalization;
using System.Text;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Reporting;

/// <summary>Формує CSV-звіт за замовленнями. Нічого не пише на диск — повертає рядок.</summary>
public static class OrderCsvReport
{
    /// <summary>Будує CSV із заголовком id;total;status.</summary>
    /// <param name="orders">Замовлення для звіту.</param>
    /// <param name="totalOf">Функція суми замовлення (наприклад, OrderService.TotalOf).</param>
    /// <returns>Текст CSV, по рядку на замовлення.</returns>
    public static string BuildCsv(IReadOnlyList<Order> orders, Func<Order, decimal> totalOf)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(totalOf);

        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine("id;total;status");
        foreach (Order order in orders)
        {
            text.AppendLine(culture, $"{order.Id};{totalOf(order):F2};{order.Status}");
        }

        return text.ToString();
    }
}
