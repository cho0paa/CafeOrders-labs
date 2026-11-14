using System;
using System.Collections.Generic;

namespace CafeOrders.Legacy;

public enum ClientKind { Regular, Vip, Staff }

public enum DocState { Draft, New, Paid, Closed }

public sealed class Visitor
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public ClientKind Kind { get; init; }
    public int DoneCount { get; init; }
    public DateTime SinceUtc { get; init; }

    public bool HasValidEmail =>
        Email is not null && Email.Contains("@");

    public bool IsLoyal =>
        DoneCount > PricingRules.LoyalOrders;
}

public class BillLine
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyBillProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(int docId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<BillLine>? items, DocState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        string? error = Validate(items, state, clientMail);
        if (error != null)
        {
            return error;
        }

        _log.Add("ok " + docId);

        _tmpSum = Subtotal(items!);
        _tmpDiscount = _discounts.For(_tmpSum, clientKind, clientDone);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = _report.Build(
            docId, clientName, items!, currency, _tmpDiscount, ship, total);

        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }
        return txt;
    }

    private static decimal Subtotal(IReadOnlyList<BillLine> items)
    {
        decimal sum = 0m;
        foreach (var it in items) sum += it.Qty * it.Price;
        return sum;
    }

    private static string? Validate(
        List<BillLine>? items, DocState state, string? clientMail)
    {
        if (items is null) return "ERR: null";
        if (items.Count == 0) return "ERR: empty";
        if (state is not (DocState.New or DocState.Paid))
            return "ERR: state";
        if (clientMail is null || !clientMail.Contains("@"))
            return "ERR: mail";
        return null;
    }

    public decimal Preview(ClientKind clientKind, int clientDone,
        List<BillLine> items)
    {
        decimal subtotal = Subtotal(items);
        decimal payable = subtotal - _discounts.For(subtotal, clientKind, clientDone);
        return payable + _shipping.For(payable);
    }

    public string DescribeClient(Visitor c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.IsLoyal) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
