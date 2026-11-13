namespace CafeOrders.Core.Legacy;

public sealed class BillItem
{
    public string MenuCode { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
