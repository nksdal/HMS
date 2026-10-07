using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.Property(h => h.Name).IsRequired().HasMaxLength(150);
        builder.Property(h => h.Address).IsRequired().HasMaxLength(250);
        builder.Property(h => h.City).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Country).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Description).HasMaxLength(1000);

        builder.HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Managers)
            .WithOne(m => m.Hotel)
            .HasForeignKey(m => m.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
