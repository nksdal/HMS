using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
        builder.Property(r => r.PricePerNight).HasColumnType("decimal(10,2)");
        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(r => new { r.HotelId, r.RoomNumber }).IsUnique();

        builder.HasMany(r => r.Reservations)
            .WithOne(res => res.Room)
            .HasForeignKey(res => res.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
