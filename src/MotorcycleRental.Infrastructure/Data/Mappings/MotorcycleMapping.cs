using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class MotorcycleMapping : IEntityTypeConfiguration<Motorcycle>
    {
        public void Configure(EntityTypeBuilder<Motorcycle> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.LicensePlate).IsUnique();
            builder.Property(e => e.LicensePlate).HasMaxLength(10).IsRequired();
            builder.Property(e => e.Model).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Year).IsRequired();
        }
    }
}
