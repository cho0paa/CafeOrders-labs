using CafeOrders.Core.Abstractions;

namespace CafeOrders.App;

/// <summary>Сповіщення в консоль — замість SMTP у навчальному застосунку.</summary>
internal sealed class ConsoleNotifier : INotifier
{
    /// <inheritdoc/>
    public void Notify(int guestId, string message) =>
        Console.WriteLine($"[сповіщення гостю {guestId}] {message}");
}
