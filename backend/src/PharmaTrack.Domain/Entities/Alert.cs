using PharmaTrack.Domain.Common;
using PharmaTrack.Domain.Enums;

namespace PharmaTrack.Domain.Entities;

public class Alert : BaseEntity
{
    public int AlertId { get; set; }
    public int MedicineId { get; set; }
    public int? BatchId { get; set; }
    public AlertType AlertType { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public string? Explanation { get; set; }
    public bool IsAcknowledged { get; set; } = false;
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }

    public Medicine Medicine { get; set; } = null!;
    public Batch? Batch { get; set; }
}