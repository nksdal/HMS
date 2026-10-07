using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data.Seed;

public static class HmsDbSeeder
{
    public static async Task SeedAsync(HmsDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        // The only Admin account is created here - never through public registration.
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var (adminHash, adminSalt) = passwordHasher.HashPassword("Admin@12345");

            context.Users.Add(new User
            {
                FirstName = "System",
                LastName = "Administrator",
                PersonalNumber = "00000000000",
                PhoneNumber = "+995500000000",
                Email = "admin@hms.com",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Role = UserRole.Admin,
                EmailConfirmed = true,
                IsActive = true
            });

            await context.SaveChangesAsync();
        }

        if (!await context.Hotels.AnyAsync())
        {
            var hotel = new Hotel
            {
                Name = "HMS Grand Hotel",
                Address = "123 Main Street",
                City = "Tbilisi",
                Country = "Georgia",
                Description = "Flagship demo hotel for the HMS project."
            };

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            context.Rooms.AddRange(
                new Room { RoomNumber = "101", Type = RoomType.Single, PricePerNight = 60m, Capacity = 1, HotelId = hotel.Id },
                new Room { RoomNumber = "102", Type = RoomType.Double, PricePerNight = 90m, Capacity = 2, HotelId = hotel.Id },
                new Room { RoomNumber = "201", Type = RoomType.Suite, PricePerNight = 180m, Capacity = 4, HotelId = hotel.Id }
            );

            await context.SaveChangesAsync();
        }
    }
}
