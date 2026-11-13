namespace CafeOrders.Core.Abstractions;

/// <summary>Сповіщення гостя. Один метод: сервісу не потрібно знати, лист це чи SMS.</summary>
public interface INotifier
{
    /// <summary>Надсилає гостю повідомлення.</summary>
    /// <param name="guestId">Ідентифікатор гостя.</param>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(int guestId, string message);
}
