using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("Guests");

        builder.Property(g => g.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(g => g.LastName).IsRequired().HasMaxLength(100);
        builder.Property(g => g.PersonalNumber).IsRequired().HasMaxLength(20);
        builder.Property(g => g.Email).IsRequired().HasMaxLength(150);
        builder.Property(g => g.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(g => g.IdentityDocumentNumber).HasMaxLength(50);

        builder.HasIndex(g => g.Email).IsUnique();
        builder.HasIndex(g => g.PersonalNumber).IsUnique();
    }
}
