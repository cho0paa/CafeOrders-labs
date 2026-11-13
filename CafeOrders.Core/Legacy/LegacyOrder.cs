namespace CafeOrders.Core.Legacy;

public class LegacyLine
{
    public string MenuCode { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class LegacyOrder
{
    public int Id { get; set; }
    public int Status { get; set; }
    public decimal Total { get; set; }
    public string Email { get; set; } = "";
    public List<LegacyLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyLine line) => Lines.Add(line);
}

public class ArchivedOrder : LegacyOrder
{
    public ArchivedOrder(int id) => Id = id;

    public override void AddLine(LegacyLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
