using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Medicine : BaseEntity
{
    public int MedicineId { get; set; }
    public int CategoryId { get; set; }
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public string UnitOfMeasure { get; set; } = "Box";

    public Category Category { get; set; } = null!;
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}