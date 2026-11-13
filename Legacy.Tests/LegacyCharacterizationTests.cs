using System.Globalization;

namespace CafeOrders.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел залежить від культури середовища, тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, string state, string mail, string expected)
    {
        List<BillLine>? items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<BillLine>(),
            _ => null,
        };
        var sut = new LegacyBillProcessor();

        string actual = sut.Handle(
            1001, 7, "Іван", mail, "regular", 0, items, state, "UAH", new DateTime(2026, 3, 1), false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyBillProcessor();

        string report = sut.Handle(
            1001, 7, "Іван", "a@b.c", "vip", 3, TwoLines(), "new", "UAH", new DateTime(2026, 3, 1), false);

        string expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "CAP-1 x2 = 300.00 UAH\n" +
            "CAK-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int done, decimal expected)
    {
        var sut = new LegacyBillProcessor();

        decimal total = sut.Preview(kind, done, TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_SumExactlyAtFreeShippingBorder_ChargesNoShipping()
    {
        // 2 × 500 = 1000 без знижки: умова «< 1000» хибна, доставка не нараховується
        var sut = new LegacyBillProcessor();
        var items = new List<BillLine> { new() { Code = "SET-1", Qty = 2, Price = 500m } };

        decimal total = sut.Preview("regular", 0, items);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_VipDiscountAboveCap_LimitsTo500()
    {
        // 0,15 × 4000 = 600 > 500: знижка обмежена стелею 500
        var sut = new LegacyBillProcessor();
        var items = new List<BillLine> { new() { Code = "SET-1", Qty = 8, Price = 500m } };

        decimal total = sut.Preview("vip", 0, items);

        Assert.Equal(3500m, total);
    }

    [Fact]
    public void Handle_SendMail_WritesMailToLog()
    {
        var sut = new LegacyBillProcessor();

        sut.Handle(1001, 7, "Іван", "a@b.c", "regular", 0, TwoLines(), "paid", "UAH", new DateTime(2026, 3, 1), true);

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new Visitor
        {
            Name = "  іван  ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1),
        };

        // метод залежить від системного годинника, тому очікування обчислюємо, а не вписуємо
        int years = DateTime.Now.Year - 2020;
        string expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        string actual = new LegacyBillProcessor().DescribeClient(c);

        Assert.Equal(expected, actual);
    }

    private static List<BillLine> TwoLines() => new()
    {
        new BillLine { Code = "CAP-1", Qty = 2, Price = 150m },
        new BillLine { Code = "CAK-2", Qty = 1, Price = 400m },
    };
}
