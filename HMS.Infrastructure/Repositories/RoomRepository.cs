using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(HmsDbContext context) : base(context) { }

    public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId) =>
        await _dbSet.Where(r => r.HotelId == hotelId).ToListAsync();
}
