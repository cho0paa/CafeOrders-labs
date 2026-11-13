using System.Text;
using CafeOrders.Core.Billing;
using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Reporting;

Console.OutputEncoding = Encoding.UTF8;

var bill = new Bill("A-1001", "Іваненко");
bill.AddLine("ESP-1", 3, 250m);
bill.AddLine("CRS-2", 12, 90m);

Console.WriteLine(bill.CalculateTotal(false));
Console.WriteLine(bill.CalculateTotal(true));
Console.WriteLine(bill.IsValid());
Console.WriteLine(bill.TryChangeStatus(1));
Console.Write(bill.BuildReport());

Console.WriteLine();
TotalRequest request = BuildRequestFromInput();
TotalResult result = DocumentTotalCalculator.Calculate(request);
string report = DocumentReport.TextOf(request, result);
new DocumentReportPrinter(Console.Out).Print(report);

// Поки що замовлення задано в коді; введення з консолі з'явиться в ЛР5 (барикада вводу).
static TotalRequest BuildRequestFromInput()
{
    return new TotalRequest(
        DocumentId: 1,
        Client: new ClientInfo("Іваненко", "i@ex.com", true),
        Lines: new List<OrderLine>
        {
            new("CAP-001", 12, 100m),
            new("CAK-002", 1, 250m),
        },
        CreatedAt: new DateOnly(2026, 3, 10),
        CouponCode: null,
        Status: OrderStatus.New,
        DeliveryPrice: 60m);
}
