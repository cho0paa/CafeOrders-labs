using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Documents;

/// <summary>Запит на розрахунок підсумку замовлення.</summary>
/// <param name="DocumentId">Номер замовлення.</param>
/// <param name="Client">Гість, що платить.</param>
/// <param name="Lines">Рядки замовлення.</param>
/// <param name="CreatedAt">Дата створення замовлення.</param>
/// <param name="CouponCode">Код купона або null.</param>
/// <param name="Status">Поточний стан замовлення.</param>
/// <param name="DeliveryPrice">Тариф доставки, грн.</param>
public sealed record TotalRequest(
    int DocumentId,
    ClientInfo Client,
    IReadOnlyList<OrderLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    OrderStatus Status,
    decimal DeliveryPrice);
