using CafeOrders.Core.Domain;
using CafeOrders.Core.Services;

namespace CafeOrders.App.Infrastructure;

/// <summary>Експорт короткого звіту замовлення у текстовий файл.</summary>
internal sealed class ReportExporter
{
    private readonly OrderService _service;

    /// <summary>Ініціалізує експортер сервісом розрахунку суми.</summary>
    /// <param name="service">Сервіс замовлень.</param>
    public ReportExporter(OrderService service) => _service = service;

    /// <summary>Записує номер і суму замовлення у файл.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="order">Замовлення.</param>
    public void ExportReport(string path, Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        // using звільняє файл навіть тоді, коли TotalOf кидає виняток.
        using var writer = new StreamWriter(path);
        writer.WriteLine($"Замовлення {order.Id}");
        writer.WriteLine(_service.TotalOf(order));
    }
}
