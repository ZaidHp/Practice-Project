using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Batch : BaseEntity
{
    public int BatchId { get; set; }
    public int MedicineId { get; set; }
    public int? PurchaseId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime? ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int QuantityReceived { get; set; }
    public int CurrentStock { get; set; }
    public decimal UnitPrice { get; set; }

    public Medicine Medicine { get; set; } = null!;
    public Purchase? Purchase { get; set; }
    public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}