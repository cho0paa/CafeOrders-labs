namespace CafeOrders.Core.Domain;

/// <summary>Стан замовлення кав’ярні в його життєвому циклі.</summary>
public enum OrderStatus
{
    New = 0,
    Preparing = 1,
    Served = 2,
    Paid = 3,
    Cancelled = 4,
}
