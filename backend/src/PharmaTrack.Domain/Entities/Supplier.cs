using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Supplier : BaseEntity
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}