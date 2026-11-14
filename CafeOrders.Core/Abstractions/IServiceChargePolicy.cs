namespace CafeOrders.Core.Abstractions;

/// <summary>Правило сервісного збору рахунку.</summary>
public interface IServiceChargePolicy
{
    /// <summary>Обчислює сервісний збір.</summary>
    /// <param name="amount">Сума страв, невід’ємна, грн.</param>
    /// <param name="isTableService">Чи обслуговували гостя за столиком.</param>
    /// <returns>Сервісний збір, грн.</returns>
    decimal ChargeFor(decimal amount, bool isTableService);
}
