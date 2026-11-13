using System.Text;
using CafeOrders.App;
using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Billing;
using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Pricing;
using CafeOrders.Core.Reporting;
using CafeOrders.Core.Services;
using CafeOrders.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

RunBillDemo();
Console.WriteLine();
RunCalculationDemo();
Console.WriteLine();
RunServiceDemo();

// ЛР2: модуль рахунку, приведений до стандарту (вивід звіряється з docs/lab02/before.txt).
static void RunBillDemo()
{
    var bill = new Bill("A-1001", "Іваненко");
    bill.AddLine("ESP-1", 3, 250m);
    bill.AddLine("CRS-2", 12, 90m);

    Console.WriteLine(bill.CalculateTotal(false));
    Console.WriteLine(bill.CalculateTotal(true));
    Console.WriteLine(bill.IsValid());
    Console.WriteLine(bill.TryChangeStatus(1));
    Console.Write(bill.BuildReport());
}

// ЛР3: розрахунок підсумку запитом і друк чека окремою командою.
static void RunCalculationDemo()
{
    TotalRequest request = BuildRequestFromInput();
    TotalResult result = DocumentTotalCalculator.Calculate(request);
    string report = DocumentReport.TextOf(request, result);
    new DocumentReportPrinter(Console.Out).Print(report);
}

// ЛР4: композиційний корінь — єдине місце, де створюються конкретні реалізації.
static void RunServiceDemo()
{
    // Таблиця відповідності замість ланцюга if у логіці: новий тип гостя — новий рядок тут.
    var policies = new Dictionary<string, IPricingPolicy>
    {
        ["regular"] = new DiscountPricingPolicy(0.05m),
        ["vip"] = new VipPricingPolicy(),
        ["staff"] = new DiscountPricingPolicy(0.30m),
    };
    const string guestType = "regular";
    IPricingPolicy pricing = policies.TryGetValue(guestType, out IPricingPolicy? found)
        ? found
        : new RegularPricingPolicy();

    var repository = new InMemoryOrderRepository();
    var service = new OrderService(repository, pricing, new ConsoleNotifier());

    var order = new Order(1, 100, DateTimeOffset.Now);
    order.AddLine(new OrderLine("CAP-001", 2, 250m));
    service.Place(order);

    Console.WriteLine($"Сума: {service.TotalOf(order):0.00}");
    Console.Write(OrderCsvReport.BuildCsv(repository.GetAll(), service.TotalOf));
}

// Поки що замовлення задано в коді; введення з консолі з’явиться в ЛР5 (барикада вводу).
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
