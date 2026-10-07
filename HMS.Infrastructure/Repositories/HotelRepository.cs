using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
{
    public HotelRepository(HmsDbContext context) : base(context) { }

    public async Task<Hotel?> GetWithRoomsAsync(int hotelId) =>
        await _dbSet.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == hotelId);
}
