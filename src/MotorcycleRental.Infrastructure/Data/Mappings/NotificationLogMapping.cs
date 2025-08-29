using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class NotificationLogMapping : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Message).HasMaxLength(1000).IsRequired();

            builder.HasOne(e => e.Motorcycle)
                .WithMany()
                .HasForeignKey(e => e.MotorcycleId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
