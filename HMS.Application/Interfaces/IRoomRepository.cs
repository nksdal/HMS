using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
}
