using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Purchase : BaseEntity
{
    public int PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
}