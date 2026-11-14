using System.Globalization;
using CafeOrders.Core.Billing;

namespace CafeOrders.Tests;

// Характеризаційні тести рахунку з ЛР2: фіксують поточну поведінку разом із боргом
// (знижка за кількістю рядків, а не порцій; форматування залежить від культури).
public class BillTests
{
    public BillTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Theory]
    [InlineData(false, 2196.0)]
    [InlineData(true, 1976.40)]
    public void CalculateTotal_SampleBill_MatchesLab2Output(bool isRegular, double expected)
    {
        Bill bill = SampleBill();

        Assert.Equal((decimal)expected, bill.CalculateTotal(isRegular));
    }

    [Fact]
    public void CalculateTotal_LargeBillNotRegular_TakesFifteenPercent()
    {
        var bill = new Bill("A-2", "Петренко");
        bill.AddLine("SET-1", 10, 600m);

        Assert.Equal(6120m, bill.CalculateTotal(false));
    }

    [Fact]
    public void CalculateTotal_MoreThanTenLines_SubtractsBulkDiscount()
    {
        var bill = new Bill("A-3", "Коваль");
        for (int i = 0; i < 11; i++)
        {
            bill.AddLine("ESP-" + i, 1, 10m);
        }

        Assert.Equal(12m, bill.CalculateTotal(false));
    }

    [Fact]
    public void TryChangeStatus_AllowedAndForbidden_FollowsTable()
    {
        Bill bill = SampleBill();

        Assert.False(bill.TryChangeStatus(2));
        Assert.True(bill.TryChangeStatus(1));
        Assert.True(bill.TryChangeStatus(2));
        Assert.False(bill.TryChangeStatus(3));
        Assert.Equal(2, bill.Status);
    }

    [Fact]
    public void TryChangeStatus_NewToCancelled_Allowed()
    {
        Bill bill = SampleBill();

        Assert.True(bill.TryChangeStatus(3));
    }

    [Theory]
    [InlineData("A-1", "Іваненко", true)]
    [InlineData("", "Іваненко", false)]
    [InlineData("A-1", "Ів", false)]
    public void IsValid_ChecksIdAndName(string id, string name, bool expected)
    {
        var bill = new Bill(id, name);
        bill.AddLine("ESP-1", 1, 10m);

        Assert.Equal(expected, bill.IsValid());
    }

    [Fact]
    public void BuildReport_SampleBill_ListsLinesAndTotal()
    {
        string report = SampleBill().BuildReport();

        Assert.Contains("Страва: ESP-1; кількість: 3; ціна: 250; сума: 750", report, StringComparison.Ordinal);
        Assert.EndsWith("Разом: 2196.0\n", report, StringComparison.Ordinal);
    }

    [Fact]
    public void FindById_ExistingAndMissing_ReturnsBillOrNull()
    {
        Bill bill = SampleBill();
        var bills = new List<Bill> { bill };

        Assert.Same(bill, Bill.FindById(bills, "A-1001"));
        Assert.Null(Bill.FindById(bills, "Z-0"));
        Assert.Equal("Іваненко", bill.GuestName);
    }

    private static Bill SampleBill()
    {
        var bill = new Bill("A-1001", "Іваненко");
        bill.AddLine("ESP-1", 3, 250m);
        bill.AddLine("CRS-2", 12, 90m);
        return bill;
    }
}
