using System.Globalization;
using System.Text;
using CafeOrders.Core.Domain;

// Кирилиця в консолі Windows без цього рядка виводиться «кракозябрами».
Console.OutputEncoding = Encoding.UTF8;

Order order = new()
{
    Id = 1,
    GuestId = 10,
    CreatedAt = DateTimeOffset.Now,
};

order.AddLine(new OrderLine
{
    MenuCode = "ESP-001",
    Quantity = 2,
    UnitPrice = 55.00m,
});
order.AddLine(new OrderLine
{
    MenuCode = "CRS-002",
    Quantity = 1,
    UnitPrice = 78.50m,
});

string total = order.Total()
    .ToString("F2", CultureInfo.InvariantCulture);
Console.WriteLine($"Замовлення #{order.Id}");
Console.WriteLine($"Стан: {order.Status}");
Console.WriteLine($"Позицій: {order.Lines.Count}");
Console.WriteLine($"Сума: {total}");
