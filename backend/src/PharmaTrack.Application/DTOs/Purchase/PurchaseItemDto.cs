using System.ComponentModel.DataAnnotations;

namespace PharmaTrack.Application.DTOs.Purchase;

public class PurchaseItemDto
    {
       [Required]
        public int MedicineId { get; set; }
        [Required]
        public string BatchNumber { get; set; } = string.Empty;
    
        public DateTime ManufactureDate { get; set; }
    
        [Required]
        public DateTime ExpiryDate { get; set; }
    
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int QuantityReceived { get; set; }
    
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal UnitPrice { get; set; }
    }