using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.HasKey(m => m.MedicineId);

        builder.Property(m => m.MedicineCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.MedicineCode)
            .IsUnique();

        builder.Property(m => m.MedicineName)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.HasOne(m => m.Category)
            .WithMany(c => c.Medicines)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
