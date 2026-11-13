using System.Globalization;
using System.Text;
using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Reporting;

/// <summary>Формує текст чека замовлення. Чиста функція: нічого не друкує й не змінює.</summary>
public static class DocumentReport
{
    /// <summary>Формує текст чека за запитом і результатом розрахунку.</summary>
    /// <param name="request">Запит, за яким рахували підсумок.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Багаторядковий текст чека; гроші — з крапкою незалежно від локалі.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        // Явна культура: без неї текст залежав би від налаштувань ОС і функція не була б чистою.
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(culture, $"Документ № {request.DocumentId}");
        text.AppendLine(culture, $"Клієнт: {request.Client.Name}");
        foreach (OrderLine line in request.Lines)
        {
            text.AppendLine(culture, $"{line.MenuCode} x {line.Quantity}");
        }

        text.AppendLine(culture, $"Разом: {result.Total:F2}");
        text.AppendLine(culture, $"Стан: {result.Status}");
        return text.ToString();
    }
}
