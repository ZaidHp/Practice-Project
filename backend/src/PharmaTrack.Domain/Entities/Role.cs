using PharmaTrack.Domain.Common;

namespace PharmaTrack.Domain.Entities;

public class Role : BaseEntity
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty; // Admin, Store Manager, Pharmacist
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}