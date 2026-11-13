using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Documents;

/// <summary>
/// Перевірки передумов розрахунку документа: кидають виняток до будь-яких обчислень.
/// </summary>
internal static class DocumentGuards
{
    /// <summary>Перевіряє заголовок документа: номер, ім'я та пошту гостя.</summary>
    /// <param name="documentId">Номер документа, більший за нуль.</param>
    /// <param name="clientName">Ім'я гостя, непорожнє.</param>
    /// <param name="clientEmail">Пошта гостя з символом @.</param>
    /// <exception cref="ArgumentOutOfRangeException">Номер не додатний.</exception>
    /// <exception cref="ArgumentException">Ім'я чи пошта порожні або пошта без @.</exception>
    public static void EnsureHeaderValid(int documentId, string clientName, string clientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientEmail);
        if (!clientEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(clientEmail));
        }
    }

    /// <summary>Перевіряє один рядок документа.</summary>
    /// <param name="line">Рядок із кодом позиції, кількістю й ціною.</param>
    /// <exception cref="ArgumentNullException">Рядок відсутній.</exception>
    /// <exception cref="ArgumentException">Код позиції меню порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість не додатна або ціна від'ємна.</exception>
    public static void EnsureLineValid(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.MenuCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
    }

    /// <summary>Перевіряє колекцію рядків документа та кожен рядок у ній.</summary>
    /// <param name="lines">Рядки документа, щонайменше один.</param>
    /// <exception cref="ArgumentNullException">Колекція відсутня.</exception>
    /// <exception cref="ArgumentException">Колекція порожня.</exception>
    public static void EnsureLinesValid(IReadOnlyList<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException("Документ не містить жодної позиції.", nameof(lines));
        }

        foreach (OrderLine line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
