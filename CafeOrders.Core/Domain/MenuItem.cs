namespace CafeOrders.Core.Domain;

/// <summary>
/// Позиція меню кав’ярні. Код незмінний; ціну й доступність змінюють лише методи з перевіркою.
/// </summary>
public sealed class MenuItem
{
    /// <summary>Ініціалізує позицію меню й перевіряє інваріант.</summary>
    /// <param name="code">Код позиції, непорожній, наприклад ESP-001.</param>
    /// <param name="title">Назва страви або напою, непорожня.</param>
    /// <param name="price">Ціна порції, більша за нуль, грн.</param>
    /// <param name="portionGrams">Вага (об’єм) порції, більша за нуль, г.</param>
    /// <exception cref="ArgumentException">Код або назва порожні.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Ціна або вага не додатні.</exception>
    public MenuItem(string code, string title, decimal price, int portionGrams)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(portionGrams);

        Code = code;
        Title = title;
        Price = price;
        PortionGrams = portionGrams;
        IsAvailable = true;
    }

    /// <summary>Отримує код позиції меню.</summary>
    public string Code { get; }

    /// <summary>Отримує назву страви або напою.</summary>
    public string Title { get; }

    /// <summary>Отримує поточну ціну порції, грн.</summary>
    public decimal Price { get; private set; }

    /// <summary>Отримує вагу (об’єм) порції в грамах.</summary>
    public int PortionGrams { get; }

    /// <summary>Отримує ознаку, що позицію можна замовити.</summary>
    public bool IsAvailable { get; private set; }

    /// <summary>Змінює ціну позиції; вже створені рядки замовлень зберігають стару ціну.</summary>
    /// <param name="newPrice">Нова ціна, більша за нуль.</param>
    public void ChangePrice(decimal newPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newPrice);
        Price = newPrice;
    }

    /// <summary>Знімає позицію з продажу (наприклад, закінчився інгредієнт).</summary>
    public void Disable() => IsAvailable = false;

    /// <summary>Повертає позицію в продаж.</summary>
    public void Enable() => IsAvailable = true;
}
