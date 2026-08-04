using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Category : BaseEntity
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}