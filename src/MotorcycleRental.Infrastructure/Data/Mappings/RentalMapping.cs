using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class RentalMapping : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.DailyRate);
            builder.Property(e => e.TotalCost);
            builder.Property(e => e.Plan).HasConversion<string>();

            builder.HasOne(e => e.Motorcycle)
                .WithMany(e => e.Rentals)
                .HasForeignKey(e => e.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DeliveryDriver)
                .WithMany(e => e.Rentals)
                .HasForeignKey(e => e.DeliveryDriverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
