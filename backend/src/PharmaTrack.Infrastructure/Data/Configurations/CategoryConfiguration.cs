using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.CategoryName).IsUnique();
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}