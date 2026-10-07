using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(HmsDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);

    public async Task<bool> ExistsByPersonalNumberAsync(string personalNumber) =>
        await _dbSet.AnyAsync(u => u.PersonalNumber == personalNumber);

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber) =>
        await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber);
}
