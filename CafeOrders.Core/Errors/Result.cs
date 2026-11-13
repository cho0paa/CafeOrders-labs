namespace CafeOrders.Core.Errors;

// Фабрики винесено з Result<T> у неузагальнений клас (CA1000): виклик Result.Ok(line)
// виводить тип сам, а Result<OrderLine>.Ok(line) змушував би повторювати його.

/// <summary>Фабрика результатів операцій з очікуваною помилкою.</summary>
public static class Result
{
    /// <summary>Створює успішний результат.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення.</param>
    /// <returns>Успішний результат.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Створює невдалий результат.</summary>
    /// <typeparam name="T">Тип значення, якого немає.</typeparam>
    /// <param name="error">Пояснення помилки для користувача.</param>
    /// <returns>Невдалий результат.</returns>
    public static Result<T> Fail<T>(string error) => new(false, default, error);
}
