using System.Text;
using CafeOrders.Core.Billing;

Console.OutputEncoding = Encoding.UTF8;

var bill = new Bill("A-1001", "Іваненко");
bill.AddLine("ESP-1", 3, 250m);
bill.AddLine("CRS-2", 12, 90m);

Console.WriteLine(bill.CalculateTotal(false));
Console.WriteLine(bill.CalculateTotal(true));
Console.WriteLine(bill.IsValid());
Console.WriteLine(bill.TryChangeStatus(1));
Console.Write(bill.BuildReport());
