using System.Text.Json;

namespace CafeOrders.Core.Legacy;

// Оточення класу-Бога: конкретні класи, які OrderManager створює сам у полях.
public class FileOrderStore
{
    public void SaveToFile(LegacyOrder order, string path)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(order));
    }
}

public class SmtpMailer
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine("SMTP -> " + to + ": " + subject + " / " + body);
    }
}
