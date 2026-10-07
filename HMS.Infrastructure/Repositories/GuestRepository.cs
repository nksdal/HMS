using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class GuestRepository : GenericRepository<Guest>, IGuestRepository
{
    public GuestRepository(HmsDbContext context) : base(context) { }

    public async Task<Guest?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(g => g.Email == email);
}
