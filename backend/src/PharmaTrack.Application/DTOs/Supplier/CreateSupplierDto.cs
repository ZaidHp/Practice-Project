using System.ComponentModel.DataAnnotations;

namespace PharmaTrack.Application.DTOs.Supplier;

public class CreateSupplierDto
{
    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(150, ErrorMessage = "Supplier name cannot exceed 150 characters.")]
    public string SupplierName { get; set; } = string.Empty;
    
    public string? ContactPerson { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    public string? Address { get; set; }
}