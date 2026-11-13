namespace CafeOrders.Core.Documents;

/// <summary>Дані гостя, який платить за замовлення.</summary>
/// <param name="Name">Ім’я гостя.</param>
/// <param name="Email">Пошта гостя.</param>
/// <param name="IsRegular">Чи є гість постійним.</param>
public sealed record ClientInfo(string Name, string Email, bool IsRegular);
