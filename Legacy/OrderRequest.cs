namespace CafeOrders.Legacy;

public sealed record OrderRequest(
    int DocId,
    Visitor Client,
    IReadOnlyList<BillLine>? Items,
    DocState State,
    string Currency,
    bool SendMail);
