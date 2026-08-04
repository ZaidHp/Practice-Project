using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public Role Role { get; set; } = null!;
    public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
}