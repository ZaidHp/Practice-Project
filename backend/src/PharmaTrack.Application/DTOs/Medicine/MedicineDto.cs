namespace PharmaTrack.Application.DTOs.Medicine;

public class MedicineDto
{
    public int MedicineId { get; set; }
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ReorderLevel { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int TotalActiveStock { get; set; }
    
    public bool IsActive { get; set; }
}