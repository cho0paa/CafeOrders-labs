namespace CafeOrders.Core.Billing;

/// <summary>
/// Рахунок гостя кав’ярні: рядки зі стравами, стан і підсумок до сплати.
/// </summary>
public class Bill
{
    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному гостю, грн.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному гостю.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на великий рахунок, грн.</summary>
    private const decimal LargeBillThreshold = 5000m;

    /// <summary>Частка знижки на великий рахунок.</summary>
    private const decimal LargeBillDiscountRate = 0.15m;

    /// <summary>Кількість рядків, понад яку діє знижка за велике замовлення.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума знижки за велике замовлення, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Межа кількості рядків у рахунку (не включно).</summary>
    private const int MaxLineCount = 100;

    /// <summary>Найменша допустима довжина імені гостя.</summary>
    private const int MinGuestNameLength = 3;

    /// <summary>Стан «новий».</summary>
    private const int StatusNew = 0;

    /// <summary>Стан «оплачений».</summary>
    private const int StatusPaid = 1;

    /// <summary>Стан «поданий».</summary>
    private const int StatusServed = 2;

    /// <summary>Стан «скасований».</summary>
    private const int StatusCancelled = 3;

    private readonly List<string[]> _lines = new List<string[]>();

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
    public static Bill? FindById(List<Bill> bills, string billId)
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
        decimal total = 0m;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total += quantity * unitPrice;
            lineCount++;
        }

        // Знижки не сумуються: постійному гостю — своя, решті — лише за великий рахунок.
        if (isRegularGuest && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeBillThreshold)
        {
            total *= 1m - LargeBillDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0m)
        {
            total = 0m;
        }

        // ПДВ нараховується на суму вже після всіх знижок.
        total += total * VatRate;
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
        if (Status == StatusNew && newStatus == StatusPaid)
        {
            Status = StatusPaid;
            return true;
        }

        if (Status == StatusPaid && newStatus == StatusServed)
        {
            Status = StatusServed;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
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
        if (Id is not null && Id.Length > 0
            && GuestName is not null
            && GuestName.Length > MinGuestNameLength - 1
            && _lines.Count > 0
            && _lines.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
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
