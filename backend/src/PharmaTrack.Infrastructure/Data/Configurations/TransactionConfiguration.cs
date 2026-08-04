using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
	public void Configure(EntityTypeBuilder<StockTransaction> builder)
	{
		builder.HasKey(t => t.TransactionId);

		builder.Property(t => t.TransactionType)
			.HasConversion<string>();

		builder.HasQueryFilter(t => !t.IsDeleted);

		builder.HasOne(t => t.Batch)
			.WithMany(b => b.Transactions)
			.HasForeignKey(t => t.BatchId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(t => t.User)
			.WithMany(u => u.Transactions)
			.HasForeignKey(t => t.UserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}