using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(r => r.RoleName).IsUnique();
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}