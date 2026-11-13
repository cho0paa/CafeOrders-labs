namespace CafeOrders.Core.Errors;

/// <summary>Результат операції з очікуваною помилкою: або значення, або пояснення.</summary>
/// <typeparam name="T">Тип значення в разі успіху.</typeparam>
public readonly struct Result<T> : IEquatable<Result<T>>
{
    /// <summary>Ініціалізує результат; створюйте через Result.Ok і Result.Fail.</summary>
    /// <param name="isSuccess">Ознака успіху.</param>
    /// <param name="value">Значення при успіху.</param>
    /// <param name="error">Пояснення помилки.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Отримує ознаку успіху.</summary>
    public bool IsSuccess { get; }

    /// <summary>Отримує значення; має сенс лише при IsSuccess.</summary>
    public T? Value { get; }

    /// <summary>Отримує пояснення помилки для користувача; порожнє при успіху.</summary>
    public string Error { get; }

    /// <summary>Порівнює два результати.</summary>
    /// <param name="left">Лівий результат.</param>
    /// <param name="right">Правий результат.</param>
    /// <returns>true, якщо результати рівні.</returns>
    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    /// <summary>Порівнює два результати на нерівність.</summary>
    /// <param name="left">Лівий результат.</param>
    /// <param name="right">Правий результат.</param>
    /// <returns>true, якщо результати різні.</returns>
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(Result<T> other) =>
        IsSuccess == other.IsSuccess
        && EqualityComparer<T?>.Default.Equals(Value, other.Value)
        && Error == other.Error;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(IsSuccess, Value, Error);
}
