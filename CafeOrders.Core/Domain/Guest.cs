namespace CafeOrders.Core.Domain;

/// <summary>Незмінний запис довідника гостей кав’ярні.</summary>
public sealed record Guest
{
    /// <summary>Ініціалізує запис гостя й перевіряє інваріант.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="name">Ім’я гостя, непорожнє.</param>
    /// <param name="email">Пошта гостя з символом @.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор не додатний.</exception>
    /// <exception cref="ArgumentException">Ім’я порожнє або пошта без @.</exception>
    public Guest(int id, string name, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("Пошта гостя має містити символ @.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    /// <summary>Отримує ідентифікатор гостя.</summary>
    public int Id { get; }

    /// <summary>Отримує ім’я гостя.</summary>
    public string Name { get; }

    /// <summary>Отримує пошту гостя.</summary>
    public string Email { get; }

    /// <summary>Отримує ознаку постійного гостя; задається лише під час створення.</summary>
    public bool IsRegular { get; init; }
}
