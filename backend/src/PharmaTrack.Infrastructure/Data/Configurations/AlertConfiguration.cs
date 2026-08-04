using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.Property(a => a.ConfidenceScore).HasPrecision(18,2);
        builder.Property(a => a.AlertType).
        HasConversion<string>();
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}