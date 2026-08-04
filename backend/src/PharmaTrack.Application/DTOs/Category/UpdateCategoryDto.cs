using System.ComponentModel.DataAnnotations;

namespace PharmaTrack.Application.DTOs.Category;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}