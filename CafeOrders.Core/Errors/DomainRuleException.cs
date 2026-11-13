namespace CafeOrders.Core.Errors;

/// <summary>Порушено правило предметної області кав’ярні.</summary>
public class DomainRuleException : Exception
{
    /// <summary>Ініціалізує виняток без повідомлення.</summary>
    public DomainRuleException()
        : this("unknown", "Порушено правило предметної області.")
    {
    }

    /// <summary>Ініціалізує виняток з повідомленням.</summary>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string message)
        : this("unknown", message)
    {
    }

    /// <summary>Ініціалізує виняток з повідомленням і внутрішнім винятком.</summary>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    /// <param name="innerException">Причина.</param>
    public DomainRuleException(string message, Exception innerException)
        : this("unknown", message, innerException)
    {
    }

    /// <summary>Ініціалізує виняток з ідентифікатором правила.</summary>
    /// <param name="rule">Ідентифікатор правила, наприклад order.editable.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Ініціалізує виняток з ідентифікатором правила й причиною.</summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Повідомлення українською.</param>
    /// <param name="innerException">Причина.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        Rule = rule;
    }

    /// <summary>Отримує ідентифікатор порушеного правила, наприклад order.editable.</summary>
    public string Rule { get; } = "unknown";
}
