namespace CafeOrders.Core.Legacy;

public class OrderManager
{
    private readonly FileOrderStore _store = new FileOrderStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyOrder order, string guestType)
    {
        decimal total = 0m;
        foreach (var line in order.Lines)
            total += line.UnitPrice * line.Quantity;

        if (guestType == "regular")
            total = total * 0.95m;
        else if (guestType == "vip")
            total = total * 0.90m;
        else if (guestType == "staff")
            total = total * 0.70m;

        if (total > 1000m)
            total = total - 50m;

        order.Total = total;
        order.Status = 1;
        _store.SaveToFile(order, "orders.json");
        _mailer.Send(order.Email, "Order " + order.Id, "" + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyOrder> orders)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in orders)
            text += o.Id + ";" + o.Total + ";" + o.Status;

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyOrder order)
    {
        LegacyOrder archived = new ArchivedOrder(order.Id);
        foreach (var line in order.Lines)
            archived.AddLine(line);

        _store.SaveToFile(archived, "archive.json");
    }
}
