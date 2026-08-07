using System.ComponentModel.DataAnnotations;

namespace PharmaTrack.Application.DTOs.Purchase;

 public class CreatePurchaseRequestDto
    {
        [Required]
        public int SupplierId { get; set; }
    
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;
    
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal TotalAmount { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    
        [Required]
        [MinLength(1, ErrorMessage = "At least one medicine must be included in the purchase.")]
        public List<PurchaseItemDto> Medicines { get; set; } = new();
    }