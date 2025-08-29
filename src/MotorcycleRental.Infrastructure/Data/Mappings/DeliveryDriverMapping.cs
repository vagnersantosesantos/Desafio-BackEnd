using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class DeliveryDriverMapping : IEntityTypeConfiguration<DeliveryDriver>
    {
        public void Configure(EntityTypeBuilder<DeliveryDriver> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.CNPJ).IsUnique();
            builder.HasIndex(e => e.LicenseNumber).IsUnique();
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.CNPJ).HasMaxLength(14).IsRequired();
            builder.Property(e => e.LicenseNumber).HasMaxLength(20).IsRequired();
            builder.Property(e => e.LicenseImagePath).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.LicenseType).HasConversion<string>().HasMaxLength(5);
        }
    }
}
