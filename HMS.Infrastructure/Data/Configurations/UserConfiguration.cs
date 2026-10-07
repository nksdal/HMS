using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PersonalNumber).IsRequired().HasMaxLength(20);
        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.PasswordSalt).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.EmailVerificationTokenHash).HasMaxLength(200);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PersonalNumber).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.HasOne(u => u.Guest)
            .WithMany()
            .HasForeignKey(u => u.GuestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
