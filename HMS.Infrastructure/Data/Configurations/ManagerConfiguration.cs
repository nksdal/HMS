using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.ToTable("Managers");

        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.LastName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.PersonalNumber).IsRequired().HasMaxLength(20);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(150);
        builder.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);

        builder.HasIndex(m => m.Email).IsUnique();
        builder.HasIndex(m => m.PersonalNumber).IsUnique();
    }
}
