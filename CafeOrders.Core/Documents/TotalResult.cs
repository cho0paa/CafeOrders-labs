using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Documents;

/// <summary>Результат розрахунку: підсумок і наступний стан замовлення.</summary>
/// <param name="Total">Підсумок до сплати, грн.</param>
/// <param name="Status">Наступний стан замовлення.</param>
public sealed record TotalResult(decimal Total, OrderStatus Status);
