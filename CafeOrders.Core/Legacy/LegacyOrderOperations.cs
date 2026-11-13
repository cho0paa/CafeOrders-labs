using CafeOrders.Core.Abstractions;
using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Legacy;

public class LegacyOrderOperations
{
    private readonly IOrderRepository _repo;

    public LegacyOrderOperations(IOrderRepository repo) => _repo = repo;

    // АНТИПАТЕРН 1: порожній catch, дефект зникає
    public Order? Load(int orderId)
    {
        try
        {
            return _repo.GetById(orderId);
        }
        catch (Exception)
        {
        }
        return null;
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    public void Cancel(int orderId, string reason)
    {
        try
        {
            var order = _repo.GetById(orderId);
            order!.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // АНТИПАТЕРН 4: try/catch замість звичайної умови
    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var q = int.Parse(row);
                if (q > 0) valid++;
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }
        return valid;
    }
}
