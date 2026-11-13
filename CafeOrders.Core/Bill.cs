using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeOrders.Core.Billing;

/// <summary>
/// Рахунок гостя кав’ярні: рядки зі стравами, стан і підсумок до сплати.
/// </summary>
public class Bill
{
    private readonly List<string[]> _lines = new List<string[]>();

    // public string prim;   // примітка, поки не треба

    /// <summary>
    /// Ініціалізує новий рахунок у стані «новий».
    /// </summary>
    /// <param name="id">Номер рахунку.</param>
    /// <param name="guestName">Ім’я гостя, на якого оформлено рахунок.</param>
    public Bill(string id, string guestName)
    {
        Id = id;
        GuestName = guestName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Отримує номер рахунку.</summary>
    public string Id { get; private set; }

    /// <summary>Отримує ім’я гостя.</summary>
    public string GuestName { get; private set; }

    /// <summary>
    /// Отримує код стану: 0 — новий, 1 — оплачений, 2 — поданий, 3 — скасований.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>Отримує момент створення рахунку.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає рахунок за номером у списку.
    /// </summary>
    /// <param name="bills">Список рахунків для пошуку.</param>
    /// <param name="billId">Номер рахунку, який треба знайти.</param>
    /// <returns>Знайдений рахунок або null, якщо такого номера немає.</returns>
    public static Bill FindById(List<Bill> bills, string billId)
    {
        for (int i = 0; i < bills.Count; i++)
        {
            if (bills[i].Id == billId)
            {
                return bills[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до рахунку рядок зі стравою меню.
    /// </summary>
    /// <param name="menuCode">Код позиції меню.</param>
    /// <param name="quantity">Кількість порцій.</param>
    /// <param name="unitPrice">Ціна однієї порції, грн.</param>
    public void AddLine(string menuCode, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = menuCode;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює підсумок рахунку зі знижками та ПДВ.
    /// </summary>
    /// <param name="isRegularGuest">Чи є гість постійним (дає право на знижку).</param>
    /// <returns>Сума до сплати, округлена до копійок.</returns>
    public decimal CalculateTotal(bool isRegularGuest)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total = total + (quantity * unitPrice);
            lineCount = lineCount + 1;
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
        // Знижки не сумуються: постійному гостю — своя, решті — лише за великий рахунок.
        if (isRegularGuest == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок.
        total = total + (total * 0.2m);
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан рахунку, якщо перехід дозволений правилами кав’ярні.
    /// </summary>
    /// <param name="newStatus">Код цільового стану.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений і стан не змінився.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи заповнено рахунок коректно.
    /// </summary>
    /// <returns>true, якщо номер, ім’я гостя, рядки і стан допустимі.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && GuestName != null
            && GuestName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий чек: рядок на кожну страву та підсумок без знижки постійного гостя.
    /// </summary>
    /// <returns>Текст чека, кожен рядок завершується символом нового рядка.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            decimal amount = int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2]);
            report = report + "Страва: " + _lines[i][0]
                + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2]
                + "; сума: " + amount + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
