using PharmaTrack.Domain.Enums;

namespace PharmaTrack.Application.DTOs.Transaction;

    public class TransactionDto
    {
        public int UserId { get; set; }
		public int BatchId { get; set; }
	    public TransactionType TransactionType { get; set; }
	    public int Quantity { get; set; }
	    public DateTime TransactionDate { get; set; }
	    public string? Remarks { get; set; }
	    public bool IsAdminOverride { get; set; } = false;
    }