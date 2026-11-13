namespace CafeOrders.Core.Domain;

/// <summary>Стан замовлення кав’ярні в його життєвому циклі.</summary>
public enum OrderStatus
{
    /// <summary>Замовлення щойно створене.</summary>
    New = 0,

    /// <summary>Замовлення передане на кухню.</summary>
    Preparing = 1,

    /// <summary>Страви подано гостю.</summary>
    Served = 2,

    /// <summary>Рахунок закрито.</summary>
    Paid = 3,

    /// <summary>Замовлення скасоване.</summary>
    Cancelled = 4,
}
