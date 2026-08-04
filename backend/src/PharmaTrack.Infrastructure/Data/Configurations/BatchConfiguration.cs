using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.HasKey(b => b.BatchId);

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.UnitPrice)
            .HasPrecision(18, 2);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasOne(b => b.Medicine)
            .WithMany(m => m.Batches)
            .HasForeignKey(b => b.MedicineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Purchase)
            .WithMany(p => p.Batches)
            .HasForeignKey(b => b.PurchaseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
