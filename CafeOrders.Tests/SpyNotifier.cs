using CafeOrders.Core.Abstractions;

namespace CafeOrders.Tests;

// Шпигун: запам’ятовує виклики, щоб тест перевірив взаємодію.
internal sealed class SpyNotifier : INotifier
{
    public List<string> Messages { get; } = new();

    public void Notify(int guestId, string message) => Messages.Add(message);
}
