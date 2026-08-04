using PharmaTrack.Domain.Common;
using PharmaTrack.Domain.Enums;

namespace PharmaTrack.Domain.Entities;

public class StockTransaction : BaseEntity
{
	public int TransactionId { get; set; }
	public int BatchId { get; set; }
	public int UserId { get; set; }
	public TransactionType TransactionType { get; set; }
	public int Quantity { get; set; }
	public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
	public string? Remarks { get; set; }
	public bool IsAdminOverride { get; set; } = false;

	public Batch Batch { get; set; } = null!;
	public User User { get; set; } = null!;
}