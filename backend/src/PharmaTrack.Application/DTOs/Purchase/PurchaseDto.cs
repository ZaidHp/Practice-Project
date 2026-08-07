namespace PharmaTrack.Application.DTOs.Purchase;

public class PurchaseDto
{
    public int PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
}