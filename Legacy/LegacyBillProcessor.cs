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

    public string Describe()
    {
        var s = Name.Trim().ToUpper();
        if (Kind == ClientKind.Vip) s += " [VIP]";
        if (IsLoyal) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + Email.ToLower() + ">";
        return s + " стаж " +
            (DateTime.Now.Year - SinceUtc.Year);
    }
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
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(OrderRequest req)
    {
        string? error = Validate(req);
        if (error != null)
        {
            return error;
        }

        _log.Add("ok " + req.DocId);

        decimal subtotal = Subtotal(req.Items!);
        decimal discount = _discounts.For(subtotal, req.Client);
        decimal ship = _shipping.For(subtotal - discount);
        decimal total = subtotal - discount + ship;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }

        return _report.Build(req, discount, ship, total);
    }

    private static decimal Subtotal(IReadOnlyList<BillLine> items)
    {
        decimal sum = 0m;
        foreach (var it in items) sum += it.Qty * it.Price;
        return sum;
    }

    private static string? Validate(OrderRequest req)
    {
        if (req.Items is null) return "ERR: null";
        if (req.Items.Count == 0) return "ERR: empty";
        if (req.State is not (DocState.New or DocState.Paid))
            return "ERR: state";
        if (!req.Client.HasValidEmail) return "ERR: mail";
        return null;
    }

    public decimal Preview(Visitor client, IReadOnlyList<BillLine> items)
    {
        decimal subtotal = Subtotal(items);
        decimal payable = subtotal - _discounts.For(subtotal, client);
        return payable + _shipping.For(payable);
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
