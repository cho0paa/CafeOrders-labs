using System.Globalization;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;

namespace CafeOrders.App.Input;

/// <summary>
/// Барикада: перетворює рядки, введені касиром, на рядок замовлення або пояснення помилки.
/// </summary>
internal static class OrderLineParser
{
    /// <summary>Розбирає введені код, кількість і ціну.</summary>
    /// <param name="menuCode">Код позиції меню.</param>
    /// <param name="quantity">Кількість порцій.</param>
    /// <param name="price">Ціна порції; роздільник дробової частини — крапка.</param>
    /// <returns>Рядок замовлення або пояснення, що введено не так.</returns>
    public static Result<OrderLine> Parse(string? menuCode, string? quantity, string? price)
    {
        if (string.IsNullOrWhiteSpace(menuCode))
        {
            return Result.Fail<OrderLine>("Код позиції меню порожній");
        }

        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q)
            || q <= 0)
        {
            return Result.Fail<OrderLine>("Кількість — ціле число більше 0");
        }

        // Касир вводить ціну з крапкою незалежно від мови Windows: культура фіксована.
        // Роздільник тисяч заборонено: з NumberStyles.Number рядок «55,50» читався як 5550.
        const NumberStyles priceStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        if (!decimal.TryParse(price, priceStyle, CultureInfo.InvariantCulture, out decimal p)
            || p < 0m)
        {
            return Result.Fail<OrderLine>("Ціна — число не менше 0, роздільник — крапка");
        }

        return Result.Ok(new OrderLine(menuCode.Trim(), q, p));
    }

    /// <summary>Рахує рядки, що містять додатну цілу кількість.</summary>
    /// <param name="rows">Рядки, введені касиром.</param>
    /// <returns>Кількість коректних значень.</returns>
    public static int CountValidQuantities(IEnumerable<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) && q > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
