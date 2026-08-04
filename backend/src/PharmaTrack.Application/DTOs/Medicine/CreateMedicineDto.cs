using System.ComponentModel.DataAnnotations;

namespace PharmaTrack.Application.DTOs.Medicine;

public class CreateMedicineDto
{
    [Required(ErrorMessage = "Medicine code is required")]
    [StringLength(50)]
    public string MedicineCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Medicine name is required")]
    [StringLength(150)]
    public string MedicineName { get; set; } = string.Empty;

    public string? GenericName { get; set; }

    [Required(ErrorMessage = "Category ID is required.")]
    public int CategoryId { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "Reorder level must be a positive number.")]
    public int ReorderLevel { get; set; }

    [Required(ErrorMessage = "Unit of measure is required.")]
    public string UnitOfMeasure { get; set; } = string.Empty;
}