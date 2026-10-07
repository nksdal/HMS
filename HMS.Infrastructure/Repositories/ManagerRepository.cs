using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class ManagerRepository : GenericRepository<Manager>, IManagerRepository
{
    public ManagerRepository(HmsDbContext context) : base(context) { }

    public async Task<IEnumerable<Manager>> GetByHotelIdAsync(int hotelId) =>
        await _dbSet.Where(m => m.HotelId == hotelId).ToListAsync();
}
