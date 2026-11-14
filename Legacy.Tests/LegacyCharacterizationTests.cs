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
    [InlineData(null, DocState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", DocState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", DocState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", DocState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, DocState state, string mail, string expected)
    {
        List<BillLine>? items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<BillLine>(),
            _ => null,
        };
        var sut = new LegacyBillProcessor();

        string actual = sut.Handle(
            1001, 7, "Іван", mail, ClientKind.Regular, 0, items, state, "UAH", new DateTime(2026, 3, 1), false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyBillProcessor();

        string report = sut.Handle(
            1001, 7, "Іван", "a@b.c", ClientKind.Vip, 3, TwoLines(), DocState.New, "UAH", new DateTime(2026, 3, 1), false);

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
    [InlineData(ClientKind.Regular, 0, 760)]
    [InlineData(ClientKind.Vip, 0, 655)]
    [InlineData(ClientKind.Staff, 0, 550)]
    [InlineData(ClientKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(ClientKind kind, int done, decimal expected)
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

        decimal total = sut.Preview(ClientKind.Regular, 0, items);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_VipDiscountAboveCap_LimitsTo500()
    {
        // 0,15 × 4000 = 600 > 500: знижка обмежена стелею 500
        var sut = new LegacyBillProcessor();
        var items = new List<BillLine> { new() { Code = "SET-1", Qty = 8, Price = 500m } };

        decimal total = sut.Preview(ClientKind.Vip, 0, items);

        Assert.Equal(3500m, total);
    }

    [Fact]
    public void Handle_SendMail_WritesMailToLog()
    {
        var sut = new LegacyBillProcessor();

        sut.Handle(1001, 7, "Іван", "a@b.c", ClientKind.Regular, 0, TwoLines(), DocState.Paid, "UAH", new DateTime(2026, 3, 1), true);

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new Visitor
        {
            Name = "  іван  ",
            Email = "IVAN@MAIL.COM",
            Kind = ClientKind.Vip,
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
