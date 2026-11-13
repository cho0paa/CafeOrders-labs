using System.Text;
using CafeOrders.App;
using CafeOrders.App.Input;
using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Billing;
using CafeOrders.Core.Documents;
using CafeOrders.Core.Domain;
using CafeOrders.Core.Errors;
using CafeOrders.Core.Pricing;
using CafeOrders.Core.Reporting;
using CafeOrders.Core.Services;
using CafeOrders.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

if (args.Length > 0 && args[0] == "add-line")
{
    RunAddLineDialog();
    return;
}

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

// ЛР5: введення рядків замовлення касиром; помилка вводу — повідомлення й повторний запит.
static void RunAddLineDialog()
{
    var repository = new InMemoryOrderRepository();
    var service = new OrderService(repository, new RegularPricingPolicy(), new ConsoleNotifier());
    var order = new Order(1, 100, DateTimeOffset.Now);
    service.Place(order);

    while (true)
    {
        Console.Write("Код позиції (порожній рядок — завершити): ");
        string? menuCode = ReadInput();
        if (string.IsNullOrWhiteSpace(menuCode))
        {
            break;
        }

        Console.Write("Кількість: ");
        string? quantity = ReadInput();
        Console.Write("Ціна: ");
        string? price = ReadInput();

        Result<OrderLine> parsed = OrderLineParser.Parse(menuCode, quantity, price);
        if (!parsed.IsSuccess)
        {
            Console.WriteLine($"Помилка вводу: {parsed.Error}");
            continue;
        }

        service.AddLine(order.Id, parsed.Value!);
        Console.WriteLine($"Додано: {parsed.Value!.MenuCode} x {parsed.Value.Quantity}");
    }

    Console.WriteLine($"Разом: {service.TotalOf(order):0.00}");
}

// Коли ввід перенаправлено з файлу, повторюємо його у виводі, щоб журнал сеансу був читабельним.
static string? ReadInput()
{
    string? value = Console.ReadLine();
    if (Console.IsInputRedirected)
    {
        Console.WriteLine(value);
    }

    return value;
}

// Замовлення для демонстрації розрахунку ЛР3 задано в коді.
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
